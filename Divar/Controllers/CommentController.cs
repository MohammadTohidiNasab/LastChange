﻿namespace Divar.Controllers
{
    public class CommentController : Controller
    {
        private readonly string _connectionString;

        public CommentController()
        {
            _connectionString = "Server=.; Initial Catalog=Divar; Integrated Security=True; encrypt=False";
        }

        // Create comment
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("sp_InsertComment", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Body", comment.Body);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    await command.ExecuteNonQueryAsync();
                }
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        // Comment list
        public async Task<IActionResult> Index()
        {
            var comments = new List<Comment>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT Id, Body, CreatedDate FROM Comments", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    comments.Add(new Comment
                    {
                        Id = reader.GetInt32(0),
                        Body = reader.GetString(1),
                        CreatedDate = reader.GetDateTime(2)
                    });
                }
            }

            return View(comments);
        }





        // Edit comment
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Comment comment = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT Id, Body, CreatedDate FROM Comments WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    comment = new Comment
                    {
                        Id = reader.GetInt32(0),
                        Body = reader.GetString(1),
                        CreatedDate = reader.GetDateTime(2) // اگر به آن نیاز دارید
                    };
                }
            }

            if (comment == null)
            {
                return NotFound(); // اگر کامنت پیدا نشد
            }

            return View(comment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Comment comment)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand("sp_UpdateComment", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Id", comment.Id);
                    command.Parameters.AddWithValue("@Body", comment.Body);
                    command.Parameters.AddWithValue("@UpdatedDate", DateTime.Now); // اگر زمان به‌روزرسانی را می‌خواهید

                    await command.ExecuteNonQueryAsync();
                }
                return RedirectToAction("Index");
            }
            return View(comment);
        }


    }

}