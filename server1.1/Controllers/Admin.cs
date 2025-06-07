using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;


namespace server1._1.Controllers
{
    // ===================== МОДЕЛИ ===================== //

    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public Department Department { get; set; }  

        public int PositionId { get; set; }
        public Position Position { get; set; }

        public int RoleId { get; set; }
        public UserRole Role { get; set; }

        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }


    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public ICollection<Test> Tests { get; set; } = new List<Test>();
    }

    public class EmployeeCourseAssignment
    {
        public int Id { get; set; }
        public string CourseStatus { get; set; } = "Назначен";
        public DateTime TrainingDate { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } 

        public int CourseId { get; set; }
        public Course Course { get; set; }
        public string MaterialPath { get; set; } = string.Empty;
    }

    public class Test
    {
        public int Id { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int MaxAttempts { get; set; }
        public decimal PassScorePercent { get; set; }
        public int? CourseId { get; set; }   // новое поле
        public Course? Course { get; set; }  // навигационное свойство
        public int? RelatedCourseId { get; set; }
        public Course? RelatedCourse { get; set; }
    }


    public class EmployeeTestAssignment
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }  

        public int TestId { get; set; }
        public Test Test { get; set; }          

        public string Status { get; set; } = "Назначен";
        public int AttemptNumber { get; set; }
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime? StartTime { get; set; } // Время начала
        public DateTime? EndTime { get; set; }   // Время окончания
        public int TimeLimitMinutes { get; set; }  // лимит в минутах
        public int ExtraAttempts { get; set; } = 0;
        public int MaxAttempts { get; set; }


    }


    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty; 
        public int TestId { get; set; }
        public Test Test { get; set; } = null!;
        public List<Answer> Answers { get; set; } = new(); 
    }

    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty; 
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }


    public class TestCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class TestCategoryAssignment
    {
        public int Id { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }    

        public int CategoryId { get; set; }
        public TestCategory Category { get; set; }   
    }

    public class Training
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string Status { get; set; } = "";
    }





    // ===================== КОНТЕКСТ БД ===================== //
    public class AppDbContext : DbContext
    {
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<EmployeeCourseAssignment> CourseAssignments { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<EmployeeTestAssignment> TestAssignments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Training> Trainings { get; set; }

        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<TestCategoryAssignment> TestCategoryAssignments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 Сотрудник
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany()
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Test>()
              .HasOne(t => t.RelatedCourse)
              .WithMany()
              .HasForeignKey(t => t.RelatedCourseId)
              .OnDelete(DeleteBehavior.SetNull);


            // 🔹 Назначения курсов
            modelBuilder.Entity<EmployeeCourseAssignment>()
                .HasOne(c => c.Employee)
                .WithMany()
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeCourseAssignment>()
                .HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Назначения тестов
            modelBuilder.Entity<EmployeeTestAssignment>()
                .HasOne(t => t.Employee)
                .WithMany()
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict); // или Cascade, если хочешь

            modelBuilder.Entity<EmployeeTestAssignment>()
                .HasOne(t => t.Test)
                .WithMany()
                .HasForeignKey(t => t.TestId)
                .OnDelete(DeleteBehavior.Restrict); // или Cascade, если хочешь

            // 🔹 Вопросы к тестам
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Test)
                .WithMany()
                .HasForeignKey(q => q.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Ответы к вопросам
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Привязка тестов к категориям
            modelBuilder.Entity<TestCategoryAssignment>()
                .HasOne(t => t.Test)
                .WithMany()
                .HasForeignKey(t => t.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestCategoryAssignment>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
           
            // 🔹 Обучения (Training)
            modelBuilder.Entity<Training>()
                .HasOne(t => t.Employee)
                .WithMany()
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Training>()
                .HasOne(t => t.Course)
                .WithMany()
                .HasForeignKey(t => t.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            {
                modelBuilder.Entity<Test>()
                    .HasOne(t => t.Course)
                    .WithMany(c => c.Tests)
                    .HasForeignKey(t => t.CourseId)
                    .OnDelete(DeleteBehavior.SetNull); // можно по-другому
            }


        }
    }

    // ===================== КОНТРОЛЛЕР СОТРУДНИКОВ ===================== //
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/employees
        // Возвращает список всех сотрудников с данными отделов, должностей и ролей.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Role)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = e.FullName,

                    DepartmentId = e.Department.Id,
                    Department = e.Department.Name,

                    PositionId = e.Position.Id,
                    Position = e.Position.Title,

                    RoleId = e.Role.Id,
                    Role = e.Role.Name,

                    Username = e.Username,
                    Password = e.PasswordHash
                })
                .ToListAsync();

            return Ok(employees);
        }

        // PUT: /api/employees/{id}/reset-password
        [HttpPut("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return BadRequest("Пароль не может быть пустым.");

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("Сотрудник не найден.");

            employee.PasswordHash = HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return Ok("Пароль сброшен.");
        }


        // GET: /api/employees/{id}
        // Возвращает данные по конкретному сотруднику.
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Role)
                .Where(e => e.Id == id)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = e.FullName,

                    DepartmentId = e.Department.Id,
                    Department = e.Department.Name,

                    PositionId = e.Position.Id,
                    Position = e.Position.Title,

                    RoleId = e.Role.Id,
                    Role = e.Role.Name,

                    Username = e.Username
                })
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound("Сотрудник не найден.");

            return Ok(employee);
        }

        // POST: /api/employees
        // Добавляет нового сотрудника, используя данные из CreateEmployeeDto.
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] CreateEmployeeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Все поля обязательны для заполнения.");
            }

            var department = await _context.Departments.FindAsync(dto.DepartmentId);
            var position = await _context.Positions.FindAsync(dto.PositionId);
            var role = await _context.UserRoles.FindAsync(dto.RoleId);

            if (department == null || position == null || role == null)
                return BadRequest("Указаны некорректные ID роли, должности или отдела.");

            var employee = new Employee
            {
                FullName = dto.FullName,
                DepartmentId = dto.DepartmentId,
                PositionId = dto.PositionId,
                RoleId = dto.RoleId,
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password)
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok("Сотрудник добавлен.");
        }

        // DELETE: /api/employees/{id}
        // Удаляет сотрудника по его ID.
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("Сотрудник не найден.");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok("Сотрудник удалён.");
        }

        // PUT: /api/employees/{id}
        // Обновляет данные сотрудника. Если какое-либо поле не передано (null),
        // то остаётся прежнее значение.
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("Сотрудник не найден.");

            employee.FullName = dto.FullName ?? employee.FullName;
            employee.DepartmentId = dto.DepartmentId ?? employee.DepartmentId;
            employee.PositionId = dto.PositionId ?? employee.PositionId;
            employee.RoleId = dto.RoleId ?? employee.RoleId;
            employee.Username = dto.Username ?? employee.Username;

            if (!string.IsNullOrEmpty(dto.Password))
                employee.PasswordHash = HashPassword(dto.Password);

            await _context.SaveChangesAsync();
            return Ok("Данные обновлены.");
        }

        // Приватный метод для хеширования пароля с использованием SHA256.
        private string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }
    }
    // ===================== контроллера авторизации ===================== //
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Username == dto.Username);

            if (user == null || user.PasswordHash != Hash(dto.Password))
                return Unauthorized("Неверный логин или пароль");

            return Ok(new
            {
                Role = user.Role.Name,
                EmployeeId = user.Id
            });
        }

        private string Hash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }
    }
    // ===================== Контроллер ролей ===================== //
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        // Получить все роли
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetAll()
        {
            var roles = await _context.UserRoles
                .Select(r => new UserRoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();

            return Ok(roles);
        }

        // Добавить новую роль
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] CreateRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Название роли не может быть пустым.");

            bool exists = await _context.UserRoles.AnyAsync(r => r.Name.ToLower() == dto.Name.Trim().ToLower());
            if (exists)
                return Conflict("Роль с таким названием уже существует.");

            var role = new UserRole { Name = dto.Name.Trim() };

            _context.UserRoles.Add(role);
            await _context.SaveChangesAsync();
            return Ok("Роль успешно добавлена.");
        }

        // Удалить роль по ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var role = await _context.UserRoles.FindAsync(id);
            if (role == null)
                return NotFound("Роль не найдена.");

            bool isRoleInUse = await _context.Employees.AnyAsync(e => e.RoleId == id);
            if (isRoleInUse)
                return BadRequest("Невозможно удалить роль, так как она назначена сотрудникам.");

            _context.UserRoles.Remove(role);
            await _context.SaveChangesAsync();
            return Ok("Роль успешно удалена.");
        }
    }

    // DTO для создания роли
    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
    }

    // DTO для просмотра роли
    public class UserRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // ===================== Контроллер отделов ===================== //
    [Route("api/departments")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }

        // Получить все отделы
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
        {
            var departments = await _context.Departments
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .ToListAsync();

            return Ok(departments);
        }

        // Добавить новый отдел
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] CreateDepartmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Название отдела не может быть пустым.");

            bool exists = await _context.Departments
                .AnyAsync(d => d.Name.ToLower() == dto.Name.Trim().ToLower());
            if (exists)
                return Conflict("Отдел с таким названием уже существует.");

            var department = new Department { Name = dto.Name.Trim() };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return Ok("Отдел успешно добавлен.");
        }

        // Удалить отдел по ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound("Отдел не найден.");

            bool hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == id);
            if (hasEmployees)
                return BadRequest("Невозможно удалить отдел, к которому привязаны сотрудники.");

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return Ok("Отдел успешно удалён.");
        }
    }

    // DTO для создания отдела
    public class CreateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
    }

    // DTO для просмотра отдела
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // ===================== Контроллер тестов ===================== //
    [Route("api/tests")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        // Получить список всех тестов
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestDto>>> GetAll()
        {
            var tests = await _context.Tests
                .Select(t => new TestDto
                {
                    Id = t.Id,
                    TestName = t.TestName,
                    MaxAttempts = t.MaxAttempts,
                    PassScorePercent = t.PassScorePercent
                })
                .ToListAsync();

            return Ok(tests);
        }

        // Добавить тест
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateTestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TestName) || dto.MaxAttempts <= 0 || dto.PassScorePercent <= 0)
                return BadRequest("Поля заполнены некорректно.");

            var test = new Test
            {
                TestName = dto.TestName,
                MaxAttempts = dto.MaxAttempts,
                PassScorePercent = dto.PassScorePercent
            };

            _context.Tests.Add(test);
            await _context.SaveChangesAsync();

            return Ok("Тест добавлен.");
        }

        // Обновить тест
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTestDto dto)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null) return NotFound("Тест не найден.");

            test.TestName = dto.TestName ?? test.TestName;
            test.MaxAttempts = dto.MaxAttempts ?? test.MaxAttempts;
            test.PassScorePercent = dto.PassScorePercent ?? test.PassScorePercent;

            await _context.SaveChangesAsync();
            return Ok("Тест обновлён.");
        }

        // Удалить тест
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null) return NotFound("Тест не найден.");

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
            return Ok("Тест удалён.");
        }
    }

    // ===================== Контроллер вопросов ===================== //
    [ApiController]
    [Route("api/questions")]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/questions/by-test/{testId}
        // Возвращает список вопросов для заданного теста.
        // Теперь в поле CorrectOption возвращается буква "A", "B" или "C".
        [HttpGet("by-test/{testId}")]
        public async Task<ActionResult<List<QuestionDetailsDto>>> GetByTest(int testId)
        {
            // 1) Сначала загружаем вопросы в оперативную память
            var questionsFromDb = await _context.Questions
                .Where(q => q.TestId == testId)
                .Include(q => q.Answers)
                .ToListAsync(); // <-- материализация

            // 2) Теперь уже в памяти делаем Select(...)
            var questionsDto = questionsFromDb.Select(q =>
            {
                // Сортируем варианты ответов по Id
                var orderedAnswers = q.Answers.OrderBy(a => a.Id).ToList();

                // Определяем букву, если соответствующий ответ IsCorrect
                string correctLetter = "";
                if (orderedAnswers.Count >= 1 && orderedAnswers[0].IsCorrect) correctLetter = "A";
                if (orderedAnswers.Count >= 2 && orderedAnswers[1].IsCorrect) correctLetter = "B";
                if (orderedAnswers.Count >= 3 && orderedAnswers[2].IsCorrect) correctLetter = "C";
                if (orderedAnswers.Count >= 4 && orderedAnswers[3].IsCorrect) correctLetter = "D";


                return new QuestionDetailsDto
                {
                    Id = q.Id,
                    QuestionText = q.Text,
                    TestId = q.TestId,
                    OptionA = (orderedAnswers.Count >= 1 ? orderedAnswers[0].Text : ""),
                    OptionB = (orderedAnswers.Count >= 2 ? orderedAnswers[1].Text : ""),
                    OptionC = (orderedAnswers.Count >= 3 ? orderedAnswers[2].Text : ""),
                    OptionD = (orderedAnswers.Count >= 4 ? orderedAnswers[3].Text : ""),

                    CorrectOption = correctLetter
                };
            })
            .ToList();

            return Ok(questionsDto);
        }

        [HttpGet("by-test/{testId}/count")]
        public async Task<ActionResult<int>> GetQuestionCount(int testId)
        {
            int count = await _context.Questions.CountAsync(q => q.TestId == testId);
            return Ok(count);
        }

        // POST: /api/questions
        // Создаёт новый вопрос с вариантами ответов.
        // Ожидается, что dto.CorrectOption содержит "A", "B" или "C".
        [HttpPost]
        public async Task<IActionResult> Create(QuestionDetailsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.QuestionText) ||
                string.IsNullOrWhiteSpace(dto.OptionA) ||
                string.IsNullOrWhiteSpace(dto.OptionB) ||
                string.IsNullOrWhiteSpace(dto.OptionC) ||
                string.IsNullOrWhiteSpace(dto.CorrectOption))
            {
                return BadRequest("Все поля вопроса и вариантов должны быть заполнены.");
            }

            // Создаем новый вопрос
            var question = new Question
            {
                Text = dto.QuestionText,
                TestId = dto.TestId,
                Answers = new List<Answer>
{
    new Answer { Text = dto.OptionA, IsCorrect = dto.CorrectOption == "A" },
    new Answer { Text = dto.OptionB, IsCorrect = dto.CorrectOption == "B" },
    new Answer { Text = dto.OptionC, IsCorrect = dto.CorrectOption == "C" },
    new Answer { Text = dto.OptionD, IsCorrect = dto.CorrectOption == "D" } // ← добавлено
}

            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return Ok("Вопрос создан.");
        }

        [HttpPost("import/{testId}")]
        public async Task<IActionResult> ImportFromText(int testId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не предоставлен.");

            using var reader = new StreamReader(file.OpenReadStream());
            var lines = new List<string>();
            while (!reader.EndOfStream)
                lines.Add(await reader.ReadLineAsync() ?? "");

            int count = 0;
            for (int i = 0; i + 5 < lines.Count; i += 6)
            {
                var questionText = lines[i].Trim();
                var optionA = lines[i + 1].Trim();
                var optionB = lines[i + 2].Trim();
                var optionC = lines[i + 3].Trim();
                var optionD = lines[i + 4].Trim();
                var correctLetter = lines[i + 5].Trim().ToUpper();

                if (string.IsNullOrWhiteSpace(questionText) || !"ABCD".Contains(correctLetter))
                    continue;

                var question = new Question
                {
                    Text = questionText,
                    TestId = testId,
                    Answers = new List<Answer>
            {
                new Answer { Text = optionA, IsCorrect = correctLetter == "A" },
                new Answer { Text = optionB, IsCorrect = correctLetter == "B" },
                new Answer { Text = optionC, IsCorrect = correctLetter == "C" },
                new Answer { Text = optionC, IsCorrect = correctLetter == "D" }
            }
                };

                _context.Questions.Add(question);
                count++;
            }

            await _context.SaveChangesAsync();
            return Ok($"{count} вопросов импортировано.");
        }



        // DELETE: /api/questions/{id}
        // Удаляет вопрос и его ответы.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (question == null)
                return NotFound("Вопрос не найден.");

            _context.Answers.RemoveRange(question.Answers);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return Ok("Вопрос удалён.");
        }

        // PUT: /api/questions/{id}
        // Обновляет вопрос и варианты ответов.
        // Требуется, чтобы все поля dto были заполнены, а CorrectOption – равна "A", "B" или "C".
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, QuestionDetailsDto dto)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (question == null)
                return NotFound("Вопрос не найден.");

            if (string.IsNullOrWhiteSpace(dto.QuestionText) ||
                string.IsNullOrWhiteSpace(dto.OptionA) ||
                string.IsNullOrWhiteSpace(dto.OptionB) ||
                string.IsNullOrWhiteSpace(dto.OptionC) ||
                string.IsNullOrWhiteSpace(dto.CorrectOption))
            {
                return BadRequest("Все поля должны быть заполнены.");
            }

            question.Text = dto.QuestionText;

            // Удаляем старые варианты ответов и создаем новые
            _context.Answers.RemoveRange(question.Answers);
            question.Answers = new List<Answer>
{
    new Answer { Text = dto.OptionA, IsCorrect = dto.CorrectOption == "A" },
    new Answer { Text = dto.OptionB, IsCorrect = dto.CorrectOption == "B" },
    new Answer { Text = dto.OptionC, IsCorrect = dto.CorrectOption == "C" },
    new Answer { Text = dto.OptionD, IsCorrect = dto.CorrectOption == "D" }
};


            await _context.SaveChangesAsync();
            return Ok("Вопрос обновлён.");
        }
    }


    // ===================== Контроллер ответов ===================== //
    [ApiController]
    [Route("api/answers")]
    public class AnswersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnswersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{questionId}")]
        public async Task<ActionResult<List<AnswerDto>>> GetAnswers(int questionId)
        {
            var answers = await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .Select(a => new AnswerDto
                {
                    Id = a.Id,
                    Text = a.Text,
                    IsCorrect = a.IsCorrect,
                    QuestionId = a.QuestionId
                }).ToListAsync();

            return Ok(answers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnswer(AnswerDto dto)
        {
            var answer = new Answer
            {
                Text = dto.Text,
                IsCorrect = dto.IsCorrect,
                QuestionId = dto.QuestionId
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    // ===================== Контроллер НазначенияТестов ===================== //
    [ApiController]
    [Route("api/test-assignments")]
    public class TestAssignmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TestAssignmentsController(AppDbContext context) => _context = context;

        // Назначить тест
        [HttpPost("assign")]
        public async Task<IActionResult> AssignTest([FromBody] TestAssignmentDto dto)
        {
            if (dto == null || dto.EmployeeId <= 0 || dto.TestId <= 0 || dto.TimeLimitMinutes <= 0)
                return BadRequest("Некорректные данные.");

            var employee = await _context.Employees.FindAsync(dto.EmployeeId);
            if (employee == null)
                return NotFound($"Сотрудник с Id={dto.EmployeeId} не найден.");

            var test = await _context.Tests.FindAsync(dto.TestId);
            if (test == null)
                return NotFound($"Тест с Id={dto.TestId} не найден.");

            // Проверка на дублирование
            bool exists = await _context.TestAssignments
                .AnyAsync(a => a.EmployeeId == dto.EmployeeId && a.TestId == dto.TestId);

            if (exists)
                return Conflict("Этот тест уже назначен данному сотруднику.");

            var assignment = new EmployeeTestAssignment
            {
                EmployeeId = dto.EmployeeId,
                TestId = dto.TestId,
                Deadline = dto.Deadline,
                TimeLimitMinutes = dto.TimeLimitMinutes,
                Status = "Назначен",
                AttemptNumber = 0,
                Score = 0,
                AttemptDate = DateTime.MinValue
            };

            _context.TestAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok(new { assignment.Id });
        }

        // Получить все назначения
        [HttpGet]
        public async Task<IActionResult> GetAssignments()
        {
            var list = await _context.TestAssignments
                .Include(a => a.Employee)
                .Include(a => a.Test)
                .Select(a => new TestAssignmentViewDto
                {
                    AssignmentId = a.Id,
                    Employee = a.Employee.FullName,
                    Test = a.Test.TestName,
                    Status = a.Status,
                    Deadline = a.Deadline,
                    AttemptNumber = a.AttemptNumber,
                    Score = a.Score,
                    TimeLimitMinutes = a.TimeLimitMinutes
                })
                .ToListAsync();

            return Ok(list);
        }



        // Обновить назначение теста
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, [FromBody] UpdateTestAssignmentDto dto)
        {
            var assignment = await _context.TestAssignments.FindAsync(id);
            if (assignment == null)
                return NotFound("Назначение теста не найдено.");

            if (!string.IsNullOrEmpty(dto.Status))
                assignment.Status = dto.Status;

            if (dto.AttemptNumber.HasValue)
                assignment.AttemptNumber = dto.AttemptNumber.Value;

            if (dto.Score.HasValue)
                assignment.Score = dto.Score.Value;

            if (dto.AttemptDate.HasValue)
                assignment.AttemptDate = dto.AttemptDate.Value;

            if (dto.Deadline.HasValue)
                assignment.Deadline = dto.Deadline.Value;

            if (dto.TimeLimitMinutes.HasValue)
                assignment.TimeLimitMinutes = dto.TimeLimitMinutes.Value;

            if (dto.TestId.HasValue)
                assignment.TestId = dto.TestId.Value;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Назначение теста обновлено." });
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartTest(int id)
        {
            var assignment = await _context.TestAssignments
                .Include(a => a.Test)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
                return NotFound("Назначение теста не найдено.");

            // Если тест привязан к курсу — проверяем, пройден ли он
            if (assignment.Test?.RelatedCourseId != null)
            {
                var courseCheck = await _context.CourseAssignments.FirstOrDefaultAsync(x =>
                    x.EmployeeId == assignment.EmployeeId &&
                    x.CourseId == assignment.Test.RelatedCourseId &&
                    x.CourseStatus == "Пройден");

                if (courseCheck == null)
                    return BadRequest("Нельзя начать тест: обучение по соответствующему курсу не завершено.");
            }

            if (assignment.Status == "Пройден")
                return BadRequest("Тест уже пройден.");

            assignment.Status = "В процессе";
            assignment.AttemptDate = DateTime.Now;
            assignment.AttemptNumber += 1;

            await _context.SaveChangesAsync();

            return Ok("Тест начат.");
        }



        [HttpPost("{id}/add-attempt")]
        public async Task<IActionResult> AddAttempt(int id)
        {
            var assignment = await _context.TestAssignments.FindAsync(id);
            if (assignment == null)
                return NotFound("Назначение теста не найдено.");

            assignment.ExtraAttempts += 1;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Дополнительная попытка добавлена." });
        }



        [HttpPut("{id}/link-course")]
        public async Task<IActionResult> LinkCourseToTest(int id, [FromBody] LinkCourseDto dto)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null)
                return NotFound("Тест не найден.");

            var course = await _context.Courses.FindAsync(dto.CourseId);
            if (course == null)
                return NotFound("Курс не найден.");

            test.RelatedCourseId = dto.CourseId;
            await _context.SaveChangesAsync();

            return Ok("Курс успешно привязан.");
        }





        // Завершить тест
        [HttpPost("{id}/finish")]
        public async Task<IActionResult> FinishTest(int id, [FromBody] FinishTestDto dto)
        {
            var assignment = await _context.TestAssignments
                .Include(a => a.Test)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
                return NotFound("Назначение теста не найдено.");

            if (assignment.Status != "В процессе")
                return BadRequest("Тест не находится в процессе.");

            assignment.Score = dto.Score;
            assignment.AttemptNumber += 1;

            int totalQuestions = await _context.Questions.CountAsync(q => q.TestId == assignment.TestId);
            decimal percent = totalQuestions > 0 ? (assignment.Score / (decimal)totalQuestions) * 100m : 0m;

            assignment.Status = percent >= assignment.Test.PassScorePercent ? "Пройден" : "Не пройден";
            assignment.EndTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Тест завершён.",
                AssignmentId = assignment.Id,
                Status = assignment.Status,
                Score = assignment.Score
            });
        }

        // Сброс теста
        [HttpPost("{id}/reset")]
        public async Task<IActionResult> ResetTest(int id)
        {
            var assignment = await _context.TestAssignments.FindAsync(id);
            if (assignment == null)
                return NotFound("Назначение теста не найдено.");

            assignment.Status = "Назначен";
            assignment.AttemptNumber = 0;
            assignment.Score = 0;
            assignment.AttemptDate = DateTime.MinValue;

            // 🔹 Обнуляем время начала и окончания
            assignment.StartTime = null;
            assignment.EndTime = null;

            await _context.SaveChangesAsync();

            return Ok("Тест сброшен в состояние 'Назначен'.");
        }


        // Получить тесты по сотруднику
        [HttpGet("by-employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployee(int employeeId)
        {
            var list = await _context.TestAssignments
                .Where(a => a.EmployeeId == employeeId)
                .Include(a => a.Test)
                .Select(a => new EmployeeTestAssignmentDto
                {
                    AssignmentId = a.Id,
                    TestId = a.TestId,
                    TestName = a.Test.TestName,
                    MaxAttempts = a.Test.MaxAttempts,
                    Status = a.Status,
                    Deadline = a.Deadline,
                    AttemptNumber = a.AttemptNumber,
                    Score = a.Score,
                    TimeLimitMinutes = a.TimeLimitMinutes,
                    ExtraAttempts = a.ExtraAttempts // ← ДОБАВЬ ЭТО
                })
                .ToListAsync();

            return Ok(list);
        }

        // Удалить назначение
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var assignment = await _context.TestAssignments.FindAsync(id);
            if (assignment == null)
                return NotFound("Назначение теста не найдено.");

            _context.TestAssignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return Ok("Назначение теста удалено.");
        }

        // Отчёт по тестированиям
        [HttpGet("report")]
        public async Task<IActionResult> GetReport([FromQuery] int? employeeId, [FromQuery] int? testId,
                                                   [FromQuery] string? status, [FromQuery] DateTime? from,
                                                   [FromQuery] DateTime? to)
        {
            var query = _context.TestAssignments
                .Include(a => a.Employee)
                .Include(a => a.Test)
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId);
            if (testId.HasValue)
                query = query.Where(a => a.TestId == testId);
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);
            if (from.HasValue)
                query = query.Where(a => a.Deadline >= from);
            if (to.HasValue)
                query = query.Where(a => a.Deadline <= to);

            var result = await query.Select(a => new ReportRowDto
            {
                Employee = a.Employee.FullName,
                Test = a.Test.TestName,
                Status = a.Status,
                Deadline = a.Deadline,
                Attempt = a.AttemptNumber,
                MaxAttempts = a.Test.MaxAttempts,
                ExtraAttempts = a.ExtraAttempts,
                Score = a.Score,
                TotalQuestions = _context.Questions.Count(q => q.TestId == a.TestId),
                TimeLimitMinutes = a.TimeLimitMinutes,
                TimeSpentSeconds = a.StartTime.HasValue && a.EndTime.HasValue
         ? (int?)(a.EndTime.Value - a.StartTime.Value).TotalSeconds
         : null
            }).ToListAsync();








            return Ok(result);
        }
    }

    // DTO для назначения теста (POST)
    public class ReportRowDto
    {
        public string Employee { get; set; } = string.Empty;
        public string Test { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }

        public int Attempt { get; set; }
        public int MaxAttempts { get; set; }
        public int ExtraAttempts { get; set; }

        public int Score { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }

        public int TimeLimitMinutes { get; set; }
        public int? TimeSpentMinutes { get; set; }
        public int? TimeSpentSeconds { get; set; }

        // Для отображения
        public string AttemptDisplay { get; set; } = ""; // отображение попыток "2 / 11"
        public string ScoreDisplay => $"{CorrectAnswers} / {TotalQuestions}";
        public string TimeSpentFormatted =>
            (TimeSpentSeconds.HasValue && TimeSpentSeconds.Value >= 0)
            ? $"{TimeSpentSeconds.Value / 60:D2}:{TimeSpentSeconds.Value % 60:D2}"
            : "00:00";

    }


    public class TestAssignmentDto
    {
        public int EmployeeId { get; set; }
        public int TestId { get; set; }
        public DateTime Deadline { get; set; }
        public int TimeLimitMinutes { get; set; }  
    }


    // DTO для обновления назначения теста (PUT)
    public class UpdateTestAssignmentDto
    {
        public string? Status { get; set; }
        public int? Score { get; set; }
        public int? AttemptNumber { get; set; }
        public DateTime? AttemptDate { get; set; }

        // 🆕 Новые свойства:
        public DateTime? Deadline { get; set; }
        public int? TimeLimitMinutes { get; set; }
        public int? TestId { get; set; }
    }


    // DTO для завершения теста (POST /finish)
    public class FinishTestDto
    {
        public int Score { get; set; }
        // Флаг успешного завершения теста: true – тест пройден, false – не пройден.
    }

    // ===================== Контроллер что то с тестами ===================== //

    [ApiController]
    [Route("api/test-category-assignments")]
    public class TestCategoryAssignmentController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TestCategoryAssignmentController(AppDbContext db) => _db = db;

        // GET /api/test-category-assignments/by-category/{categoryId}
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var list = await _db.TestCategoryAssignments
                .Where(a => a.CategoryId == categoryId)
                .Include(a => a.Test)
                .Select(a => new CategoryAssignmentView
                {
                    AssignmentId = a.Id,
                    TestId = a.TestId,
                    TestName = a.Test.TestName
                })
                .ToListAsync();

            return Ok(list);
        }

        // POST /api/test-category-assignments
        [HttpPost]
        public async Task<IActionResult> Assign([FromBody] CategoryAssignmentDto dto)
        {
            // защита от дублей
            bool exists = await _db.TestCategoryAssignments
                .AnyAsync(a => a.CategoryId == dto.CategoryId && a.TestId == dto.TestId);
            if (exists)
                return BadRequest("Эта привязка уже существует.");

            _db.TestCategoryAssignments.Add(new TestCategoryAssignment
            {
                CategoryId = dto.CategoryId,
                TestId = dto.TestId
            });
            await _db.SaveChangesAsync();
            return Ok();
        }

        // DELETE /api/test-category-assignments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var a = await _db.TestCategoryAssignments.FindAsync(id);
            if (a == null) return NotFound();
            _db.TestCategoryAssignments.Remove(a);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }

    // ===================== Контроллер категорий теста ===================== //

    [ApiController]
    [Route("api/test-categories")]
    public class TestCategoryController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TestCategoryController(AppDbContext db) => _db = db;

        // GET: /api/test-categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
          => Ok(await _db.TestCategories.ToListAsync());

        // POST: /api/test-categories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CategoryName))
                return BadRequest("Нужно указать имя категории.");
            _db.TestCategories.Add(new TestCategory { CategoryName = dto.CategoryName.Trim() });
            await _db.SaveChangesAsync();
            return Ok();
        }

        // PUT: /api/test-categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Rename(int id, [FromBody] UpdateCategoryDto dto)
        {
            var cat = await _db.TestCategories.FindAsync(id);
            if (cat == null) return NotFound();
            cat.CategoryName = dto.CategoryName?.Trim() ?? cat.CategoryName;
            await _db.SaveChangesAsync();
            return Ok();
        }

        // DELETE: /api/test-categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.TestCategories.FindAsync(id);
            if (cat == null) return NotFound();
            _db.TestCategories.Remove(cat);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }

    // ===================== Контроллер курсов ===================== //

    [ApiController]
    [Route("api/courses")]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        // Получить все курсы
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
        {
            var courses = await _context.Courses
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title
                })
                .ToListAsync();

            return Ok(courses);
        }

        // Добавить курс с проверкой на уникальность
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] CourseDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Название курса не может быть пустым.");

            // Проверка: есть ли уже курс с таким названием
            bool exists = await _context.Courses.AnyAsync(c => c.Title.ToLower() == dto.Title.ToLower());
            if (exists)
                return Conflict("Курс с таким названием уже существует.");

            var course = new Course
            {
                Title = dto.Title
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Обновить курс
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Название курса не может быть пустым.");

            // Проверка: нельзя обновить на название, которое уже занято другим курсом
            bool exists = await _context.Courses.AnyAsync(c => c.Id != id && c.Title.ToLower() == dto.Title.ToLower());
            if (exists)
                return Conflict("Курс с таким названием уже существует.");

            course.Title = dto.Title;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Удалить курс
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }



    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }


    // ===================== Контроллер обучение курсов ===================== //
    [ApiController]
    [Route("api/[controller]")]
    public class CourseAssignmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CourseAssignmentsController(AppDbContext context)
        {
            _context = context;
        }

        // Получить все назначения
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseAssignmentViewDto>>> GetAll()
        {
            var assignments = await _context.CourseAssignments
                .Include(a => a.Employee)
                .Include(a => a.Course)
                .ToListAsync();

            return assignments.Select(a => new CourseAssignmentViewDto
            {
                Id = a.Id,
                Employee = a.Employee.FullName,
                Course = a.Course.Title,
                CourseName = a.Course.Title,
                CourseStatus = a.CourseStatus,
                TrainingDate = a.TrainingDate,
                MaterialPath = a.MaterialPath  // ← добавить это
            }).ToList();

        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> MarkAsCompleted(int id)
        {
            var assignment = await _context.CourseAssignments.FindAsync(id);
            if (assignment == null) return NotFound();

            assignment.CourseStatus = "Пройден";
            await _context.SaveChangesAsync();
            return Ok();
        }


        // Назначить курс
        [HttpPost("assign")]
        public async Task<ActionResult> Assign([FromBody] CourseAssignmentDto dto)
        {
            // Проверка на существование такого назначения
            bool exists = await _context.CourseAssignments
                .AnyAsync(a => a.EmployeeId == dto.EmployeeId && a.CourseId == dto.CourseId);

            if (exists)
                return Conflict("Этот курс уже назначен данному сотруднику.");

            var assignment = new EmployeeCourseAssignment
            {
                EmployeeId = dto.EmployeeId,
                CourseId = dto.CourseId,

                CourseStatus = dto.CourseStatus,
                TrainingDate = dto.TrainingDate,
                MaterialPath = dto.MaterialPath
            };

            _context.CourseAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok();
        }



        // Обновить назначение
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CourseAssignmentDto dto)
        {
            var assignment = await _context.CourseAssignments.FindAsync(id);
            if (assignment == null) return NotFound();

            // Обновляем только переданные поля
            if (dto.CourseId > 0) assignment.CourseId = dto.CourseId;
            assignment.TrainingDate = dto.TrainingDate;
            assignment.CourseStatus = dto.CourseStatus;
            assignment.MaterialPath = dto.MaterialPath;

            await _context.SaveChangesAsync();
            return Ok();
        }



        [HttpGet("by-employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<CourseAssignmentViewDto>>> GetByEmployee(int employeeId)
        {
            var assignments = await _context.CourseAssignments
                .Include(a => a.Employee)
                .Include(a => a.Course)
                .Where(a => a.EmployeeId == employeeId)
                .ToListAsync();

            return assignments.Select(a => new CourseAssignmentViewDto
            {
                Id = a.Id,
                Employee = a.Employee.FullName,
                CourseName = a.Course.Title, // ← это работает
                TrainingDate = a.TrainingDate,
                CourseStatus = a.CourseStatus,
                MaterialPath = a.MaterialPath
            }).ToList();
        }



        // Удалить назначение
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var assignment = await _context.CourseAssignments.FindAsync(id);
            if (assignment == null) return NotFound();

            _context.CourseAssignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }



    // ===================== Контроллер должностей ===================== //
    [Route("api/positions")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
            _context = context;
        }

        // Получить все должности
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PositionDto>>> GetAll()
        {
            var positions = await _context.Positions
                .Select(p => new PositionDto
                {
                    Id = p.Id,
                    Title = p.Title
                })
                .ToListAsync();

            return Ok(positions);
        }

        // Добавить новую должность
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] CreatePositionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Название должности не может быть пустым.");

            bool exists = await _context.Positions.AnyAsync(p => p.Title.ToLower() == dto.Title.Trim().ToLower());
            if (exists)
                return Conflict("Должность с таким названием уже существует.");

            var position = new Position { Title = dto.Title.Trim() };

            _context.Positions.Add(position);
            await _context.SaveChangesAsync();
            return Ok("Должность успешно добавлена.");
        }

        // Удалить должность по ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
                return NotFound("Должность не найдена.");

            bool isInUse = await _context.Employees.AnyAsync(e => e.PositionId == id);
            if (isInUse)
                return BadRequest("Невозможно удалить должность, так как она назначена сотрудникам.");

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();
            return Ok("Должность успешно удалена.");
        }
    }

    // DTO для создания должности
    public class CreatePositionDto
    {
        public string Title { get; set; } = string.Empty;
    }

    // DTO для просмотра должности
    public class PositionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    // ===================== Очистка пустых записей ===================== //
    public static class CleanupService
    {
        public static async Task CleanEmptyRecords(AppDbContext db)
        {
            var emptyRoles = db.UserRoles.Where(r => string.IsNullOrWhiteSpace(r.Name));
            var emptyDepts = db.Departments.Where(d => string.IsNullOrWhiteSpace(d.Name));
            var emptyPositions = db.Positions.Where(p => string.IsNullOrWhiteSpace(p.Title));

            db.UserRoles.RemoveRange(emptyRoles);
            db.Departments.RemoveRange(emptyDepts);
            db.Positions.RemoveRange(emptyPositions);

            // 🔥 Очистка связанных, но уже удалённых курсов у тестов
            var testsWithInvalidCourse = await db.Tests
                .Where(t => t.RelatedCourseId != null &&
                            !db.Courses.Any(c => c.Id == t.RelatedCourseId))
                .ToListAsync();

            foreach (var test in testsWithInvalidCourse)
            {
                test.RelatedCourseId = null;
            }

            // 🔥 Очистка назначений курсов, у которых удалён сам курс
            var invalidCourseAssignments = await db.CourseAssignments
                .Where(a => !db.Courses.Any(c => c.Id == a.CourseId))
                .ToListAsync();

            db.CourseAssignments.RemoveRange(invalidCourseAssignments);

            await db.SaveChangesAsync();
        }
    }




    // ===================== Dto ===================== //
    public class CategoryAssignmentDto
    {
        public int CategoryId { get; set; }
        public int TestId { get; set; }
    }

    public class CategoryAssignmentView
    {
        public int AssignmentId { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; } = "";

        // ДОБАВИЛИ:
        public string CategoryName { get; set; } = "";
    }

    public class CreateCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
    }

    public class UpdateCategoryDto
    {
        public string? CategoryName { get; set; }
    }
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public string Department { get; set; } = string.Empty;

        public int PositionId { get; set; }
        public string Position { get; set; } = string.Empty;

        public int RoleId { get; set; }
        public string Role { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } // ← ЭТО ОБЯЗАТЕЛЬНО
    }


    public class CreateEmployeeDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue)]
        public int PositionId { get; set; }

        [Range(1, int.MaxValue)]
        public int RoleId { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }


    public class UpdateEmployeeDto
    {
        public string? FullName { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public int? RoleId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class NamedDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }


    public class CourseAssignmentDto
    {
        public int EmployeeId { get; set; }
        public int CourseId { get; set; }
        public DateTime TrainingDate { get; set; }
        public string CourseStatus { get; set; } = "Назначен";
        public string MaterialPath { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
    }



    public class CourseAssignmentViewDto
    {
        public int Id { get; set; }
        public string Employee { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;          // ← ОБЯЗАТЕЛЬНО!
        public DateTime TrainingDate { get; set; }
        public string CourseStatus { get; set; } = string.Empty;
        public string MaterialPath { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;

    }







    public class TestDto
    {
        public int Id { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int MaxAttempts { get; set; }
        public decimal PassScorePercent { get; set; }
    }

    public class CreateTestDto
    {
        public string TestName { get; set; } = string.Empty;
        public int MaxAttempts { get; set; }
        public decimal PassScorePercent { get; set; }
    }

    public class UpdateTestDto
    {
        public string? TestName { get; set; }
        public int? MaxAttempts { get; set; }
        public decimal? PassScorePercent { get; set; }
    }

    // DTO для вопроса
    public class QuestionDetailsDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int TestId { get; set; }
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty; // ← Новое поле
        public string CorrectOption { get; set; } = string.Empty; // теперь может быть "A", "B", "C" или "D"
    }





    // DTO для ответа
    public class AnswerDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
    }

    public class EmployeeTestAssignmentDto
    {
        public int AssignmentId { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int MaxAttempts { get; set; }
        public int ExtraAttempts { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public int AttemptNumber { get; set; }
        public int Score { get; set; }
        public int TimeLimitMinutes { get; set; }

        public string AttemptDisplay { get; set; } = "";
        public string ScoreDisplay { get; set; } = ""; // ← новое поле
    }


    public class TestAssignmentViewDto
    {
        public int AssignmentId { get; set; }
        public string Employee { get; set; } = "";
        public string Test { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime Deadline { get; set; }
        public int AttemptNumber { get; set; }
        public int Score { get; set; }
        public int TimeLimitMinutes { get; set; }  // лимит времени
    }

    public class LinkCourseDto
    {
        public int CourseId { get; set; }
    }







}
