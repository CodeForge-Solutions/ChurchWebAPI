using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using churchWebAPI.DBContext;
using churchWebAPI.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using churchWebAPI.DTOs;
using Newtonsoft.Json.Linq;

namespace churchWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BibleVersesController : ControllerBase
    {
        private readonly authDBContext _context;

        public BibleVersesController(authDBContext context)
        {
            _context = context;
        }

        [HttpGet("save-random-verse")]
        public async Task<IActionResult> SaveRandomVerseToDb()
        {
            using var client = new HttpClient();

            // Send a request to the Bible API to get a random verse
            var request = new HttpRequestMessage(HttpMethod.Get, "https://bible-api.com/?random=verse");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Read the response content and parse the JSON
            var content = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(content);
            var verse = jsonObject["verses"][0];

            // Extract the necessary fields
            var bookName = verse["book_name"].ToString();
            var chapter = int.Parse(verse["chapter"].ToString());
            var verseNumber = int.Parse(verse["verse"].ToString());
            var verseText = verse["text"].ToString().Replace("\n","").Trim();
            var translationId = 0;

            // Check if the verse already exists in the database
            var existingVerse = _context.trnBibleVerses
                .FirstOrDefault(b => b.BookName == bookName &&
                                     b.Chapter == chapter &&
                                     b.Verse == verseNumber &&
                                     b.TranslationId == translationId &&
                                     b.IsActive == true);

            if (existingVerse != null)
            {
                return Ok("Verse already exists in the database.");
            }

            // Create a new BibleVerse object if the verse doesn't exist
            var bibleVerse = new clsBibleVerse
            {
                BookName = bookName,
                Chapter = chapter,
                Verse = verseNumber,
                VerseText = verseText,
                TranslationId = translationId
            };

            // Add the BibleVerse to the context and save changes using LINQ
            if (verseText != "") {
                _context.trnBibleVerses.Add(bibleVerse);
                await _context.SaveChangesAsync();
            }

            return Ok(bibleVerse);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportVerses()
        {
            try
            {
                string filePath = @"C:\Users\admin\Documents\SQL Server Management Studio\ChurchWebApp\BibleVerses - Do not delete.json";

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return BadRequest("File path cannot be empty.");
                }

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                // Configure JSON serializer options to handle lowercase properties
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Handles camelCase property names
                    PropertyNameCaseInsensitive = true // Makes the property name matching case insensitive
                };

                // Read and deserialize the JSON file
                string jsonString = await System.IO.File.ReadAllTextAsync(filePath);
                var verses = JsonSerializer.Deserialize<List<BibleVerseV2>>(jsonString, options);


                if (verses == null || !verses.Any())
                {
                    return BadRequest("No verses found in the file.");
                }

                foreach (var verse in verses)
                {
                    if (!await _context.trnBibleVersesV2.AnyAsync(v => v.Verse == verse.Verse))
                    {
                        _context.trnBibleVersesV2.Add(verse);
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok("Verses imported successfully.");
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(HttpContext, ex);
                return StatusCode(500, new SuccessFailureDto { IsSuccess = false, Message = "Something Went Wrong!!" });
            }
            
        }

        //[HttpGet("saveverse")]
        //public async Task<IActionResult> SaveVerseToFile()
        //{
        //    // Initialize the HttpClient
        //    using var client = new HttpClient();

        //    // Create the HttpRequestMessage
        //    var request = new HttpRequestMessage(HttpMethod.Get, "https://bible-api.com/?random=verse");

        //    // Send the request and get the response
        //    var response = await client.SendAsync(request);
        //    response.EnsureSuccessStatusCode();

        //    // Read the response content as a string
        //    var content = await response.Content.ReadAsStringAsync();

        //    var appDirectory = AppDomain.CurrentDomain.BaseDirectory;

        //    // Set the file path in the application directory
        //    var filePath = Path.Combine(appDirectory, "BibleVerse.txt");

        //    // Create the file if it does not exist and write the content
        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        await System.IO.File.WriteAllTextAsync(filePath, content);
        //    }
        //    else
        //    {
        //        // Append to the file if it already exists
        //        await System.IO.File.AppendAllTextAsync(filePath, content);
        //    }

        //    // Write the content to a text file
        //    await System.IO.File.WriteAllTextAsync(filePath, content);

        //    return Ok("Verse saved to file successfully.");
        //}
    }
}
