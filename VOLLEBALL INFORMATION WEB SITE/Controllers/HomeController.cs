using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mini_Project2.Models;
using System.Diagnostics;
namespace Mini_Project2.Controllers
{
	public class HomeController : Controller
	{
		private readonly ClubDBContext	clubDBContext;
		private readonly PlayerDBcontext playerDBcontext;
        private readonly BoardDbContext boardDb;
        private readonly NewsDbContext _context;
        public HomeController(ClubDBContext clubDBContext, PlayerDBcontext playerDBcontext, BoardDbContext boardD, NewsDbContext context)
		{
			this.clubDBContext = clubDBContext;
			this.playerDBcontext = playerDBcontext;
            this.boardDb = boardD;
            _context = context;
        }

		public IActionResult Clubinfo(int? id)
		{
			var clubData = clubDBContext.Club.FirstOrDefault(x=>x.id == id);
			return View(clubData);
		}
        public IActionResult Ser()
        {
            return View();
        }
		[HttpPost]
        public IActionResult Serplayer(string name)
        {
			var data = playerDBcontext.Player.Where(p => p.name.Contains(name)).ToList();
            return View(data);
        }
        public IActionResult Com()
        {
			var data = playerDBcontext.Player.ToList();
            return View(data);
        }
		[HttpPost]
        public IActionResult Complayer(int? id1, int? id2)
        {
            var player1 = playerDBcontext.Player.FirstOrDefault(p => p.id == id1);
            var player2 = playerDBcontext.Player.FirstOrDefault(p => p.id == id2);
            return View(new List<Player> { player1, player2 });
        }
        public IActionResult Privacy()
		{
			return View();
		}
		public IActionResult State(int? id)
		{
			var playerData = playerDBcontext.Player.FirstOrDefault(x => x.id == id);
			return View(playerData);
		}
		public IActionResult List(int? id)
		{
			var playerData = playerDBcontext.Player.Where(p => p.team == id).ToList();
			var clubData = clubDBContext.Club.FirstOrDefault(x => x.id == id);
			var listview = new Listview
			{
				PlayerData = playerData,
				ClubData = clubData
			};
			return View(listview);
		}
		public IActionResult Index()
		{
			return View();
		}
        public IActionResult board(int? id)
        {
            if (id == null)
            {
                var boardData = boardDb.Board.ToList();
                return View(boardData);
            }
            else
            {
                TempData["id"] = id;
                var boardData = boardDb.Board.Where(p => p.Team == id).ToList();
                return View(boardData);
            }
        }

        public IActionResult Write()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Write(int id, boards board)
        {
            if (ModelState.IsValid)
            {
                board.Team = (int)TempData["id"];
                this.boardDb.Add(board);
                await boardDb.SaveChangesAsync();
                return RedirectToAction(nameof(board), new { team = board.Team });
            }
            return View(board);
        }

        public IActionResult Details(int? id) //id는 디폴트값
        {
            if (id == null || this.boardDb.Board == null)
            {
                return NotFound();  // 예외 처리
            }
            var boardData = this.boardDb.Board.FirstOrDefault(x => x.No == id);
            if (boardData == null)
            {
                return NotFound();
            }
            return View(boardData);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || this.boardDb.Board == null)
            {
                return NotFound();
            }

            var boardData = this.boardDb.Board.Find(id);

            if (boardData == null)
            {
                return NotFound();
            }
            boardData.Password = null;
            return View(boardData);
        }

        [HttpPost]
        public IActionResult Edit(int? id, boards brd)
        {
            if (id == null || this.boardDb.Board == null)
            {
                return NotFound();
            }
            // 비밀번호 검증
            var boardData = this.boardDb.Board.AsNoTracking().FirstOrDefault(x => x.No == id);
            if (boardData == null)
            {
                return NotFound();
            }
            if (brd.Password != boardData.Password)
            {
                ModelState.AddModelError("Password", "비밀번호가 일치하지 않습니다.");
            }

            if (ModelState.IsValid)
            {
                this.boardDb.Update(brd);
                this.boardDb.SaveChanges();
                return RedirectToAction("board", "Home");
            }
            return View(boardData);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardData = this.boardDb.Board.FirstOrDefault(x => x.No == id);

            if (boardData == null)
            {
                return NotFound();
            }

            return View(boardData);
        }
        [HttpPost]
        public IActionResult Delete(int? id, int Password)
        {
            if (id == null || this.boardDb.Board == null)
            {
                return NotFound();
            }

            var boardData = this.boardDb.Board.AsNoTracking().FirstOrDefault(x => x.No == id);

            if (boardData == null)
            {
                return NotFound();
            }

            if (Password != boardData.Password)
            {
                ModelState.AddModelError("Password", "비밀번호가 일치하지 않습니다.");

                return RedirectToAction("Edit", new { id = boardData.No });
            }

            this.boardDb.Board.Remove(boardData);
            this.boardDb.SaveChanges();

            return RedirectToAction("board"); // 삭제 후 인덱스 페이지로 리디렉션
        }
        [HttpPost]
        public IActionResult Search(string name)
        {
            var data = this.boardDb.Board.Where(x => x.Name.Contains(name)).ToList();
            return View(data);
        }

        public IActionResult vote()
        {
            var data = playerDBcontext.Player
                .Where(p => p.position != "COA" && p.position != "DIR")
                .OrderByDescending(p => p.vote)
                .ToList();
            return View(data);
        }
        public async Task<IActionResult> votein(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await playerDBcontext.Player.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            player.vote += 1;

            try
            {
                await playerDBcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(player.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("vote");
        }

        private bool PlayerExists(int id)
        {
            return playerDBcontext.Player.Any(e => e.id == id);
        }


        public IActionResult Heungkuk()
		{
			return View();
		}
        public IActionResult sch()
        {
            return View();
        }
        public IActionResult schdetail()
        {
            return View();
        }
        public IActionResult KoreanRoad()
		{
			return View();
		}
		public IActionResult Hyundai()
		{
			return View();
		}
		public IActionResult GS()
		{
			return View();
		}
		public IActionResult Peppers()
		{
			return View();
		}
		public IActionResult Ibk()
		{
			return View();
		}
		public IActionResult KGC()
		{
			return View();
		}

		public IActionResult HeungkukClub()
		{
			return View();
		}
		public IActionResult KoreanRoadClub()
		{
			return View();
		}
		public IActionResult HyundaiClub()
		{
			return View();
		}
		public IActionResult GSClub()
		{
			return View();
		}
		public IActionResult PeppersClub()
		{
			return View();
		}
		public IActionResult IbkClub()
		{
			return View();
		}
		public IActionResult KGCClub()
		{
			return View();
		}

        public IActionResult IndexNews()
        {
            var newsItems = _context.NewsItems.Where(x => x.Team == 1).ToList();
            return View(newsItems);
        }

        public IActionResult HEUNGKUKNews()
        {
            var newsItems = _context.NewsItems.Where(x => x.Team == 2).ToList();
            return View(newsItems);
        }

        public IActionResult EXPRESSWAYNews()
        {
            var newsItems = _context.NewsItems.Where(x => x.Team == 3).ToList();
            return View(newsItems);
        }

        public IActionResult KGCNews()
        {
            var newsItems = _context.NewsItems.Where(x => x.Team == 4).ToList();
            return View(newsItems);
        }

        public IActionResult GSNews()
        {
            var newsItems = _context.NewsItems.Where(x => x.Team == 5).ToList();
            return View(newsItems);
        }

        public IActionResult IBKNews()
        {
            var newsItems = _context.NewsItems.Where(x => x.Team == 6).ToList();
            return View(newsItems);
        }

        public IActionResult AINews()
        {
            var newsItems = _context.NewsItems.Where(x => x.Team == 7).ToList();
            return View(newsItems);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
