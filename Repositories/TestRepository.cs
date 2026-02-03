using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Application.Interfaces;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Db;
using TestingPlatform.Infrastructure.Exceptions;
using static System.Net.Mime.MediaTypeNames;

namespace TestingPlatform.Infrastructure.Repositories
{
    public class TestRepository(AppDbContext appDbContext, IMapper mapper) : ITestRepository
    {
        public async Task<IEnumerable<TestDto>> GetAllAsync(bool? isPublic, List<int> groupIds, List<int> studentIds)
        {
            await RefreshPublicationStatusesAsync();
            var tests = appDbContext.Tests
                .OrderByDescending(t => t.PublishedAt)
                .ThenBy(t => t.Title)
                .AsNoTracking()
                .AsQueryable();
            if (isPublic is not null)
                tests = tests.Where(t => t.IsPublic == isPublic);
            if (groupIds.Any())
                tests = tests.Where(t => t.Groups.Any(g => groupIds.Contains(g.Id)));
            if (studentIds.Any())
                tests = tests.Where(t => t.Students.Any(s => studentIds.Contains(s.Id)));

            return mapper.Map<IEnumerable<TestDto>>(await tests.ToListAsync());
        }

        public async Task<IEnumerable<TestDto>> GetAllForStudent(int studentId)
        {
            await RefreshPublicationStatusesAsync();

            var tests = await appDbContext.Tests
                .Where(t => t.IsPublic)
                .Where(t =>
                    t.Students.Any(s => s.Id == studentId)
                    || t.Courses.Any(c => c.Groups.Any(g => g.Students.Any(s => s.Id == studentId)))
                    || t.Projects.Any(p => p.Groups.Any(g => g.Students.Any(s => s.Id == studentId)))
                    || t.Directions.Any(d => d.Groups.Any(g => g.Students.Any(s => s.Id == studentId)))
                )
                .ToListAsync();

            return mapper.Map<IEnumerable<TestDto>>(tests);
        }
        
        public async Task<IEnumerable<TestDto>> GetTestForStudentById(int studentId)    //ыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыыы
        {
            await RefreshPublicationStatusesAsync();
            var tests = await appDbContext.Tests
                .Where(t => t.IsPublic)
                .Where(t =>
                    t.Students.Any(s => s.Id == studentId)
                    || t.Courses.Any(c => c.Groups.Any(g => g.Students.Any(s => s.Id == studentId)))
                )
                .ToListAsync();
            return mapper.Map<IEnumerable<TestDto>>(tests);
        }
        public async Task<TestDto> GetByIdAsync(int id)
        {
            await RefreshPublicationStatusesAsync();

            var test = await appDbContext.Tests
                .Include(test => test.Directions)
                .Include(test => test.Courses)
                .Include(test => test.Groups)
                .Include(test => test.Projects)
                .Include(test => test.Students)
                    .ThenInclude(student => student.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(test => test.Id == id);

            if (test == null)
            {
                throw new EntityNotFoundException("Тест не найден");
            }

            return mapper.Map<TestDto>(test);
        }

        public async Task<int> CreateAsync(TestDto testDto)
        {
            await RefreshPublicationStatusesAsync();

            var test = mapper.Map<Test>(testDto);

            var testId = await appDbContext.AddAsync(test);

            await UpdateMembersTest(test, testDto);

            await appDbContext.SaveChangesAsync();

            return testId.Entity.Id;
        }

        public async Task UpdateAsync(TestDto testDto)
        {
            await RefreshPublicationStatusesAsync();

            var test = await appDbContext.Tests.FirstOrDefaultAsync(test => test.Id == testDto.Id);

            if (test == null)
            {
                throw new EntityNotFoundException("Тест не найден");
            }

            test.Title = testDto.Title;
            test.Description = testDto.Description;
            test.IsRepeatable = testDto.IsRepeatable;
            test.Type = testDto.Type;
            test.PublishedAt = testDto.PublishedAt;
            test.Deadline = testDto.Deadline;
            test.DurationMinutes = testDto.DurationMinutes.Value;
            test.IsPublic = testDto.IsPublic;
            test.PassingScore = testDto.PassingScore;
            test.MaxAttempts = testDto.MaxAttempts.Value;

            await UpdateMembersTest(test, testDto);

            await appDbContext.SaveChangesAsync();
        }


        public async Task DeleteAsync(int id)
        {
            await RefreshPublicationStatusesAsync();

            var test = await appDbContext.Tests.FirstOrDefaultAsync(test => test.Id == id);

            if (test == null)
            {
                throw new EntityNotFoundException("Тест не найден");
            }
            appDbContext.Tests.Remove(test);
            await appDbContext.SaveChangesAsync();
        }


        public async Task<IEnumerable<TestDto>> GetTopRecentAsync(int count = 5)
        {
            await RefreshPublicationStatusesAsync();

            var tests = await appDbContext.Tests.AsNoTracking()
                .OrderByDescending(t => t.PublishedAt)
                .ThenByDescending(t => t.Id)
                .Take(count)
                .ToListAsync();

            return mapper.Map<IEnumerable<TestDto>>(tests);
        }









        private async Task UpdateMembersTest(Test test, TestDto testDto)
        {
            await RefreshPublicationStatusesAsync();

            var studentIds = testDto.Students?.Select(x => x.Id).Where(id => id > 0).Distinct().ToArray() ?? Array.Empty<int>();
            var groupIds = testDto.Groups?.Select(x => x.Id).Where(id => id > 0).Distinct().ToArray() ?? Array.Empty<int>();
            var courseIds = testDto.Courses?.Select(x => x.Id).Where(id => id > 0).Distinct().ToArray() ?? Array.Empty<int>();
            var directionIds = testDto.Directions?.Select(x => x.Id).Where(id => id > 0).Distinct().ToArray() ?? Array.Empty<int>();
            var projectIds = testDto.Projects?.Select(x => x.Id).Where(id => id > 0).Distinct().ToArray() ?? Array.Empty<int>();
            if (appDbContext.Entry(test).State == EntityState.Detached)
                appDbContext.Attach(test);

            // === Работа со студентами ===
            // Загружаем коллекцию Students, очищаем текущие связи и добавляем новые.
            await appDbContext.Entry(test).Collection(t => t.Students).LoadAsync();
            test.Students.Clear();
            if (studentIds.Length > 0)
            {
                var students = await appDbContext.Students
                    .Where(s => studentIds.Contains(s.Id))
                    .ToListAsync();
                // Добавляем студентов к тесту.
                foreach (var s in students)
                    test.Students.Add(s);
            }

            // === Работа с группами ===
            await appDbContext.Entry(test).Collection(t => t.Groups).LoadAsync();
            test.Groups.Clear();
            if (groupIds.Length > 0)
            {
                var groups = await appDbContext.Groups
                    .Where(g => groupIds.Contains(g.Id))
                    .ToListAsync();
                foreach (var g in groups)
                    test.Groups.Add(g);
            }

            // === Работа с курсами ===
            await appDbContext.Entry(test).Collection(t => t.Courses).LoadAsync();
            test.Courses.Clear();
            if (courseIds.Length > 0)
            {
                var courses = await appDbContext.Courses
                    .Where(c => courseIds.Contains(c.Id))
                    .ToListAsync();
                foreach (var c in courses)
                    test.Courses.Add(c);
            }

            // === Работа с направлениями ===
            await appDbContext.Entry(test).Collection(t => t.Directions).LoadAsync();
            test.Directions.Clear();
            if (directionIds.Length > 0)
            {
                var directions = await appDbContext.Directions
                    .Where(d => directionIds.Contains(d.Id))
                    .ToListAsync();
                foreach (var d in directions)
                    test.Directions.Add(d);
            }

            // === Работа с проектами ===
            await appDbContext.Entry(test).Collection(t => t.Projects).LoadAsync();
            test.Projects.Clear();
            if (projectIds.Length > 0)
            {
                var projects = await appDbContext.Projects
                    .Where(p => projectIds.Contains(p.Id))
                    .ToListAsync();
                foreach (var p in projects)
                    test.Projects.Add(p);
            }
        }




        private async Task RefreshPublicationStatusesAsync()
        {
            // Получаем текущее время в формате UTC (универсальное время).
            // Оно используется для проверки времени публикации и дедлайна.
            var now = DateTimeOffset.UtcNow;

            // === 1) Опубликовать всё, что должно стать публичным ===
            // Загружаем из базы тесты, которые пока не опубликованы (IsPublic == false),
            // но имеют время публикации (PublishedAt) или дедлайн (Deadline).
            var publishCandidates = await appDbContext.Tests
                .AsNoTracking() // отключаем отслеживание, т.к. данные только читаются
                .Where(t => !t.IsPublic && (t.PublishedAt != null || t.Deadline != null))
                // Выбираем только нужные поля, чтобы не грузить весь объект.
                .Select(t => new { t.Id, t.PublishedAt, t.Deadline })
                .ToListAsync();

            // Отбираем Id тех тестов, которые нужно сделать публичными:
            // - У теста есть дата публикации (PublishedAt)
            // - Дата публикации уже наступила (<= now)
            // - Дедлайн отсутствует или ещё не наступил
            var toPublishIds = publishCandidates
                .Where(x => x.PublishedAt != null
                            && x.PublishedAt <= now
                            && (x.Deadline == null || x.Deadline > now))
                .Select(x => x.Id)
                .ToList();

            // Если есть такие тесты -- обновляем их свойство IsPublic = true.
            // ExecuteUpdateAsync -- выполняет массовое обновление напрямую в БД (без загрузки объектов в память).
            if (toPublishIds.Count > 0)
                await appDbContext.Tests
                    .Where(t => toPublishIds.Contains(t.Id))
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsPublic, true));

            // === 2) Снять с публикации, если не задана дата или дедлайн истёк ===
            // Загружаем тесты, которые сейчас публичные,
            // но у них нет даты публикации (PublishedAt == null)
            // или есть дедлайн (Deadline != null).
            var unpublishCandidates = await appDbContext.Tests
                .AsNoTracking()
                .Where(t => t.IsPublic && (t.PublishedAt == null || t.Deadline != null))
                .Select(t => new { t.Id, t.PublishedAt, t.Deadline })
                .ToListAsync();

            // Отбираем Id тех тестов, которые нужно снять с публикации:
            // - Нет даты публикации (PublishedAt == null)
            // - Или дедлайн задан и уже наступил (Deadline <= now)
            var toUnpublishIds = unpublishCandidates
                .Where(x => x.PublishedAt == null
                            || (x.Deadline != null && x.Deadline <= now))
                .Select(x => x.Id)
                .ToList();

            // Если есть тесты, которые нужно скрыть -- обновляем свойство IsPublic = false.
            if (toUnpublishIds.Count > 0)
                await appDbContext.Tests
                    .Where(t => toUnpublishIds.Contains(t.Id))
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsPublic, false));
        }

        public async Task<IEnumerable<object>> GetTestCountByTypeAsync()
        {
            return await appDbContext.Tests
                .AsNoTracking()                 // Чтение без отслеживания - быстрее и экономнее по памяти
                .GroupBy(t => t.Type)           // GroupBy: группируем тесты по полю Type
                .Select(g => new                // Select: проекция каждой группы в анонимный объект
                {
                    Type = g.Key,               // g.Key -- общий ключ группы (значение Type)
                    Count = g.Count()           // Count: агрегат -- количество элементов в группе
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetCourseStatsAsync()
        {
            return await appDbContext.Courses
                .AsNoTracking()
                .Select(c => new
                {
                    Course = c.Name,
                    // Average: среднее по времени прохождения и проходному баллу
                    AvgDuration = c.Tests.Average(t => (double?)t.DurationMinutes) ?? 0,
                    AvgPassingScore = c.Tests.Average(t => (double?)t.PassingScore) ?? 0
                })
                .ToListAsync();                  // Выполнение запроса
        }

        public async Task<IEnumerable<object>> GetDirectionAveragesAsync()
        {
            return await appDbContext.Directions
                .AsNoTracking()
                .Select(d => new
                {
                    Direction = d.Name,
                    AvgPassingScore = d.Tests.Average(t => (double?)t.PassingScore) ?? 0,
                    AvgDuration = d.Tests.Average(t => (double?)t.DurationMinutes) ?? 0
                })
                .OrderByDescending(x => x.AvgPassingScore) // OrderByDescending: сортировка по убыванию среднего балла
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetTestTimelineByPublicAsync()
        {
            return await appDbContext.Tests
                .AsNoTracking()
                .Where(t => t.PublishedAt != default)                 // Where: фильтруем только тесты с валидной датой публикации
                .GroupBy(t => new                                     // GroupBy: составной ключ = (IsPublic, Год, Месяц)
                {
                    t.IsPublic,
                    Year = t.PublishedAt.Year,
                    Month = t.PublishedAt.Month
                })
                .Select(g => new                                      // Select: агрегируем размеры групп
                {
                    g.Key.IsPublic,
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count()                                  // Count: размер группы
                })
                .OrderBy(x => x.Year)                                  // OrderBy / ThenBy: сортируем по времени
                .ThenBy(x => x.Month)
                .ThenByDescending(x => x.IsPublic)                     // В пределах месяца сначала публичные
                .ToListAsync();                                        // Выполнение запроса
        }

        public async Task<IEnumerable<object>> GetTopGroupsByTestCountAsync(int top = 10)
        {
            return await appDbContext.Tests
                .AsNoTracking()
                // SelectMany -- используется для «расплющивания» вложенных коллекций.
                // У нас у каждого теста (t) есть коллекция групп (t.Groups).
                // Вместо того чтобы получить список тестов, где внутри у каждого -- список групп,
                // SelectMany превращает всё это в один общий поток объектов:
                //   [Test1, GroupA], [Test1, GroupB], [Test2, GroupA], [Test3, GroupC] и т.д.
                // То есть каждая пара «тест-группа» становится отдельной строкой результирующего набора.
                .SelectMany(t => t.Groups.Select(g => new
                {
                    Group = g.Name,
                    TestId = t.Id
                }))

                // GroupBy -- группируем полученные пары по названию группы.
                // Теперь каждая группа (g.Key) содержит все тесты, к которым она относится.
                .GroupBy(x => x.Group)

                // Select -- для каждой группы считаем количество уникальных тестов.
                // g.Select(x => x.TestId) -- берём все Id тестов этой группы.
                // Distinct -- удаляем возможные дубликаты.
                // Count -- получаем итоговое количество тестов в группе.
                .Select(g => new
                {
                    Group = g.Key,
                    TestCount = g.Select(x => x.TestId).Distinct().Count()
                })

                // OrderByDescending -- сортируем группы по количеству тестов (от большего к меньшему).
                .OrderByDescending(x => x.TestCount)

                // Take -- оставляем только первые N записей (по умолчанию top = 10).
                .Take(top)
                .ToListAsync();
        }




    }
}
