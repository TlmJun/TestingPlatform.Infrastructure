using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using static System.Net.Mime.MediaTypeNames;

namespace TestingPlatform.Infrastructure.Repositories
{
    public class TestRepository(AppDbContext appDbContext, IMapper mapper) : ITestRepository
    {
        public async Task<IEnumerable<TestDto>> GetAllAsync(bool? isPublic, List<int> groupIds, List<int> studentIds)
        {
            // Формируем базовый запрос к таблице Tests.
            // OrderByDescending - сортируем тесты по дате публикации (новые сверху),
            // ThenBy - вторичная сортировка по названию
            // AsNoTracking - отключаем отслеживание, так как данные только читаются.
            // AsQueryable - делаем запрос динамическим, чтобы добавлять фильтры по условиям.
            var tests = appDbContext.Tests
                .OrderByDescending(t => t.PublishedAt)
                .ThenBy(t => t.Title)
                .AsNoTracking()
                .AsQueryable();

            // Если задан параметр публичности, добавляем фильтр по полю IsPublic.
            // Where - добавляет условие, но запрос пока не выполняется (отложенное выполнение).
            if (isPublic is not null)
                tests = tests.Where(t => t.IsPublic == isPublic);

            // Если переданы идентификаторы групп, выбираем тесты, связанные с этими группами.
            // Any - проверяет, есть ли у теста хотя бы одна группа с подходящим Id.
            if (groupIds.Any())
                tests = tests.Where(t => t.Groups.Any(g => groupIds.Contains(g.Id)));

            // Аналогично фильтруем по студентам.
            // Выбираем тесты, где хотя бы один студент совпадает с переданным списком.
            if (studentIds.Any())
                tests = tests.Where(t => t.Students.Any(s => studentIds.Contains(s.Id)));

            return mapper.Map<IEnumerable<TestDto>>(await tests.ToListAsync());
        }

        public async Task<IEnumerable<TestDto>> GetAllForStudent(int studentId)
        {
            var tests = await appDbContext.Tests
                // Первый фильтр: выбираем только публичные тесты.
                .Where(t => t.IsPublic)

                // Второй фильтр: оставляем тесты, доступные конкретному студенту.
                // Условие объединяет несколько вариантов доступа:
                .Where(t =>
                    // Студент напрямую привязан к тесту (t.Students содержит его Id).
                    t.Students.Any(s => s.Id == studentId)

                    // Студент входит в группу, относящуюся к курсу, связанному с тестом.
                    || t.Courses.Any(c => c.Groups.Any(g => g.Students.Any(s => s.Id == studentId)))

                    // Студент входит в группу, относящуюся к проекту, связанному с тестом.
                    || t.Projects.Any(p => p.Groups.Any(g => g.Students.Any(s => s.Id == studentId)))

                    // Студент входит в группу, относящуюся к направлению, связанному с тестом.
                    || t.Directions.Any(d => d.Groups.Any(g => g.Students.Any(s => s.Id == studentId)))
                )
                .ToListAsync();

            return mapper.Map<IEnumerable<TestDto>>(tests);
        }

        public async Task<TestDto> GetByIdAsync(int id)
        {
            var test = await appDbContext.Tests
                // Include - подгружаем связанные коллекции.
                .Include(test => test.Directions)
                .Include(test => test.Courses)
                .Include(test => test.Groups)
                .Include(test => test.Projects)
                .Include(test => test.Students)
                    // ThenInclude - используется для подгрузки вложенных связей.
                    // Здесь для каждого студента загружается его связанный объект User.
                    .ThenInclude(student => student.User)

                // AsNoTracking - отключаем отслеживание изменений, так как данные нужны только для чтения.
                // Это повышает производительность и снижает нагрузку на контекст.
                .AsNoTracking()

                // FirstOrDefaultAsync -- выполняет запрос и возвращает первый найденный тест с указанным Id.
                // Если тест не найден -- возвращает null (а не выбрасывает исключение).
                .FirstOrDefaultAsync(test => test.Id == id);

            if (test == null)
            {
                throw new Exception("Тест не найден.");
            }

            return mapper.Map<TestDto>(test);
        }

        public async Task<int> CreateAsync(TestDto testDto)
        {
            var test = mapper.Map<Test>(testDto);

            // Добавляем тест в контекст базы данных.
            // AddAsync() подготавливает тест к сохранению, но сам запрос ещё не выполняется.
            var testId = await appDbContext.AddAsync(test);

            // Сохраняем все изменения в базе данных.
            await appDbContext.SaveChangesAsync();

            return testId.Entity.Id;
        }


        public async Task UpdateAsync(TestDto testDto)
        {
            // Пытаемся найти тест по его идентификатору.
            // FirstOrDefaultAsync -- возвращает первый найденный объект или null, если такого нет.
            var test = await appDbContext.Tests.FirstOrDefaultAsync(test => test.Id == testDto.Id);

            // Проверяем, найден ли тест в базе.
            // Если нет -- выбрасываем исключение, чтобы сообщить об ошибке.
            if (test == null)
            {
                throw new Exception("Тест не найден.");
            }

            // Обновляем свойства теста новыми значениями из DTO.
            // Здесь присваиваются только простые поля (без навигационных свойств).
            test.Title = testDto.Title;
            test.Description = testDto.Description;
            test.IsRepeatable = testDto.IsRepeatable;
            test.Type = testDto.Type;
            test.PublishedAt = testDto.PublishedAt;
            test.Deadline = testDto.Deadline;
            test.DurationMinutes = testDto.DurationMinutes;
            test.IsPublic = testDto.IsPublic;
            test.PassingScore = testDto.PassingScore;
            test.MaxAttempts = testDto.MaxAttempts;

            // Сохраняем изменения в базе данных.
            // EF Core отслеживает объект test, поэтому достаточно вызвать SaveChangesAsync -- 
            // EF сам сформирует SQL-запрос UPDATE только для измененных полей.
            await appDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            // Пытаемся найти тест по переданному идентификатору.
            // FirstOrDefaultAsync -- выполняет SQL-запрос SELECT с фильтром WHERE Id = id
            // и возвращает найденный объект или null, если запись отсутствует.
            var test = await appDbContext.Tests.FirstOrDefaultAsync(test => test.Id == id);

            // Проверяем, найден ли тест.
            // Если объект не найден, выбрасываем исключение с сообщением.
            if (test == null)
            {
                throw new Exception("Тест не найден.");
            }

            // Помечаем объект для удаления.
            // Remove() не выполняет SQL-запрос сразу -- он только сообщает EF Core,
            // что данный объект должен быть удален при следующем сохранении.
            appDbContext.Tests.Remove(test);

            // Сохраняем изменения в базе.
            // EF Core выполнит SQL-запрос DELETE для указанной записи.
            await appDbContext.SaveChangesAsync();
        }


        public async Task<IEnumerable<TestDto>> GetTopRecentAsync(int count = 5)
        {
            // Формируем запрос к таблице Tests.
            // AsNoTracking() - отключаем отслеживание изменений, 
            // так как данные нужны только для чтения (ускоряет выполнение запроса).
            var tests = await appDbContext.Tests.AsNoTracking()

                // OrderByDescending - сортируем тесты по дате публикации (от новых к старым).
                // Самые свежие тесты окажутся первыми.
                .OrderByDescending(t => t.PublishedAt)

                // ThenByDescending - дополнительная сортировка по Id.
                // Используется, если PublishedAt совпадает (чтобы новые записи шли раньше).
                .ThenByDescending(t => t.Id)

                // Take(count) - берём только указанное количество записей (по умолчанию 5).
                // Это эквивалент SQL-запроса с LIMIT или TOP.
                .Take(count)

                // ToListAsync() - выполняем запрос к базе данных и получаем результаты в виде списка.
                .ToListAsync();

            return mapper.Map<IEnumerable<TestDto>>(tests);
        }



    }
}
