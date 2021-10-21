using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MovieApp1.Model;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Data;
using System.Collections;
using System.Data.SqlClient;

namespace MovieApp1
{
    class Program
    {
        static bool run = true;

        static void Main(string[] args)
        {

            using (var context = new DatabaseContext())
            {
                //DeleteAllRows();
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();
                //context.Database.Migrate();

                do
                {
                    PrintMenu();
                    ReadCommand();
                } while (run);
            }
        }

        static void DeleteAllRows()
        {
            using (var context = new DatabaseContext())
            {
                var movies = context.Movies
                   .Include(a => a.ActorsList)
                   .ToList();

                foreach (var movie in movies)
                {
                    context.Remove(movie);
                }

                var actors = context.Actors
                   .Include(a => a.MoviesList)
                   .ToList();


                foreach (var actor in actors)
                {
                    context.Remove(actor);
                }

                context.SaveChanges();
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine(" =================================== ");
            Console.WriteLine(" ====| Menu |==== ");
            Console.WriteLine(" =================================== ");
            Console.WriteLine("[1] PrintActors ");
            Console.WriteLine("[2] PrintMovies ");
            Console.WriteLine("[3] AddMovie  ");
            Console.WriteLine("[4] AddActor ");
            Console.WriteLine("[5] AddActorToMovie ");
            Console.WriteLine("[6] PrintMovieCast ");
            Console.WriteLine("[7] PrintActorMovies ");
            Console.WriteLine("[8] DeleteActor ");
            Console.WriteLine("[9] DeleteMovie ");
            Console.WriteLine("[10] DeleteAllData ");
            Console.WriteLine("[11] UpdateMovieName ");
            Console.WriteLine("[12] UpdateMovieNameSP ");
            Console.WriteLine("[13] GetMovieByID ");
            Console.WriteLine("[Exit] Close program ");
        }

        static void ReadCommand()
        {
            Console.WriteLine("Insert keyword to select an option: ");

            // Get selected option
            string selected = Console.ReadLine();

            // Exit main do while loop
            if (selected.ToLower() == "exit")
            {
                run = false;
                return;
            }

            switch (selected)
            {
                case "1":
                    GetActorsMenu();
                    break;
                case "2":
                    GetMoviesMenu();
                    break;
                case "3":
                    Console.WriteLine();
                    AddMovie();
                    break;
                case "4":
                    Console.WriteLine();
                    AddActor();
                    break;
                case "5":
                    AddActorToMovieMenu();
                    break;
                case "6":
                    Console.WriteLine("Enter MovieID: ");
                    int id1 = Convert.ToInt32(Console.ReadLine());
                    PrintMovieCast(id1);
                    break;
                case "7":
                    Console.WriteLine("Enter ActorsID: ");
                    int id2 = Convert.ToInt32(Console.ReadLine());
                    PrintActorMovies(id2);
                    break;
                case "8":
                    GetActorsMenu();
                    Console.WriteLine("Enter ActorsID: ");
                    int actorId = Convert.ToInt32(Console.ReadLine());
                    DeleteActor(actorId);
                    break;
                case "9":
                    GetMoviesMenu();
                    Console.WriteLine("Enter MoviesID: ");
                    int movieId = Convert.ToInt32(Console.ReadLine());
                    DeleteMovie(movieId);
                    break;
                case "10":
                    DeleteAllRows();
                    break;
                case "11":
                    GetMoviesMenu();
                    Console.WriteLine("Enter MovieID: ");
                    int movieID1 = Convert.ToInt32(Console.ReadLine());
                    UpdateMovieName(movieID1);
                    break;
                case "12":
                    GetMoviesMenu();
                    Console.WriteLine("Enter MovieID: ");
                    int movieID = Convert.ToInt32(Console.ReadLine());
                    UpdateMovieNameSP(movieID);
                    break;
                case "13":
                    Console.WriteLine("Enter MovieID: ");
                    int movieID3 = Convert.ToInt32(Console.ReadLine());
                    GetMovieByID(movieID3);
                    break;
                default:
                    Console.WriteLine("Please enter a valid option...");
                    break;
            }
        }

        private static void GetMoviesMenu()
        {
            Console.WriteLine();
            List<Movie> listOfMovies2 = GetMovies();
            PrintMoviesToConsole(listOfMovies2);
        }

        private static void GetActorsMenu()
        {
            Console.WriteLine();
            List<Actor> listOfActors = GetActors();
            PrintActors(listOfActors);
        }

        static List<Movie> GetMovies()
        {
            Console.WriteLine("List Of Movies...");
            //throw new NotImplementedException();

            using (var context = new DatabaseContext())
            {
                var movies = context.Movies.ToList();
                return movies;
            }
        }

        static List<Actor> GetActors()
        {
            Console.WriteLine("List Of Actors...");
            //throw new NotImplementedException();

            using (var context = new DatabaseContext())
            {
                var actors = context.Actors.ToList();
                return actors;
            }
        }

        static int AddMovie()
        {
            using (var context = new DatabaseContext())
            {
                Console.WriteLine("Vendos Titullin: ");
                string title = Console.ReadLine();
                Console.WriteLine("Vendos Vitin: ");
                string year = Console.ReadLine();
                var movie = new Movie()
                {
                    Title = title,
                    Year = year
                };
                context.Add(movie);
                context.SaveChanges();
                Console.WriteLine("The movie was added!");
                Console.WriteLine("AddActorToMovie?");
                Console.WriteLine("[1] No");
                Console.WriteLine("[2] Yes");
                string select = Console.ReadLine();
                switch (select)
                {
                    case "1":
                        break;
                    case "2":
                        while (select == "2")
                        {
                            int actorId = AddActor();
                            int movieId = movie.ID;
                            AddActorToMovie(movieId, actorId);
                            Console.WriteLine("The actor was added!");
                            Console.WriteLine("AddActorToMovie?");
                            Console.WriteLine("[1] No");
                            Console.WriteLine("[2] Yes");
                            select = Console.ReadLine();
                            context.SaveChanges();
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid Value!");
                        break;
                }

                return movie.ID;
            }
        }

        static int AddActor()
        {
            Console.WriteLine("Vendos emrin e aktorit: ");
            string fname = Console.ReadLine();
            Console.WriteLine("Vendos mbiemrin e aktorit: ");
            string lname = Console.ReadLine();
            using (var context = new DatabaseContext())
            {
                var actor = context.Actors
                    .Where(a => a.FirstName == fname && a.LastName == lname)
                    .FirstOrDefault();

                if (actor == null)
                {
                    var actor1 = new Actor()
                    {
                        FirstName = fname,
                        LastName = lname
                    };
                    context.Add(actor1);
                    context.SaveChanges();
                    Console.WriteLine("The actor was added! ");
                    return actor1.ID;
                }
                else
                {
                    Console.WriteLine("The actor exists! ");
                    return actor.ID;
                }
            }
        }

        static void AddActorToMovie(int selectedMovieId, int selectedActorId)
        {
            using (var context = new DatabaseContext())
            {
                var actor = context.Actors
                   .Include(m => m.MoviesList)
                   .Where(a => a.ID == selectedActorId)
                   .FirstOrDefault();


                var movie = context.Movies
                    .Include(a => a.ActorsList)
                    .Where(m => m.ID == selectedMovieId)
                    .FirstOrDefault();

                if (actor == null)
                {
                    Console.WriteLine("The actor does not exist");
                }
                else if (movie == null)
                {
                    Console.WriteLine("The movie does not exist");
                }
                else
                {
                    actor.MoviesList.Add(movie);
                    movie.ActorsList.Add(actor);
                    context.SaveChanges();
                }
            }
        }

        static void AddActorToMovieMenu()
        {
            string select;
            int selectedMovieId;
            int selectedActorId;
            GetMoviesMenu();
            GetActorsMenu();
            Console.WriteLine("1. Add existing actor to an existing movie");
            Console.WriteLine("2. Add new actor to a new movie");
            Console.WriteLine("3. Add new actor to a existing movie");
            Console.WriteLine("4. Add existing actor to a new movie");
            select = Console.ReadLine();
            switch (select)
            {
                case "1":
                    Console.WriteLine("Enter MovieId from the above list: ");
                    selectedMovieId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter ActorId from the above list: ");
                    selectedActorId = Convert.ToInt32(Console.ReadLine());
                    AddActorToMovie(selectedMovieId, selectedActorId);
                    break;
                case "2":
                    selectedActorId = AddActor();
                    selectedMovieId = AddMovie();
                    AddActorToMovie(selectedMovieId, selectedActorId);
                    break;
                case "3":
                    Console.WriteLine("Enter MovieID");
                    selectedMovieId = Convert.ToInt32(Console.ReadLine());
                    selectedActorId = AddActor();
                    AddActorToMovie(selectedMovieId, selectedActorId);
                    break;
                case "4":
                    Console.WriteLine("Enter ActorID");
                    selectedActorId = Convert.ToInt32(Console.ReadLine());
                    selectedMovieId = AddMovie();
                    AddActorToMovie(selectedMovieId, selectedActorId);
                    break;
                default:
                    Console.WriteLine("Invalid Value...");
                    break;
            }
        }

        static void PrintMoviesToConsole(List<Movie> listOfMovies)
        {
            List<Movie> moviesList = listOfMovies;

            foreach (var movie in moviesList)
            {
                Console.WriteLine(movie.ID);
                Console.WriteLine(movie.Title);
                Console.WriteLine(movie.Year);
            }

        }

        static void PrintActors(List<Actor> listOfActors)
        {
            List<Actor> actorsList = listOfActors;

            foreach (var actor in actorsList)
            {
                Console.WriteLine($"{actor.ID}");
                Console.WriteLine(actor.FirstName);
                Console.WriteLine(actor.LastName);
            }
        }

        static void PrintMovieCast(int movieId)
        {
            using (var context = new DatabaseContext())
            {
                var movie = context.Movies
                    .Where(m => m.ID == movieId)
                    .Select(m => new
                    {
                        m.Title,
                        m.Year
                    })
                    .FirstOrDefault();

                Console.WriteLine(movie);

                var movieA = context.Movies
                    .Where(m => m.ID == movieId)
                    .Select(m => m.ActorsList)
                    .FirstOrDefault();

                foreach (var m in movieA)
                {
                    Console.WriteLine(m.ID);
                    Console.WriteLine(m.FirstName);
                    Console.WriteLine(m.LastName);
                }
            }
        }

        static void PrintActorMovies(int actorId)
        {
            using (var context = new DatabaseContext())
            {
                var actor = context.Actors
                    .Where(a => a.ID == actorId)
                    .Select(a => new
                    {
                        a.ID,
                        a.FirstName,
                        a.LastName
                    })
                    .FirstOrDefault();

                Console.WriteLine(actor);

                var actorM = context.Actors
                    .Where(a => a.ID == actorId)
                    .Select(a => a.MoviesList)
                    .FirstOrDefault();

                foreach (var a in actorM)
                {
                    Console.WriteLine(a.Title);
                    Console.WriteLine(a.Year);
                }
            }
        }

        static void DeleteActor(int actorID)
        {
            using (var context = new DatabaseContext())
            {
                var actor = context.Actors
                    .Where(a => a.ID == actorID)
                    .Include(a => a.MoviesList)
                    .FirstOrDefault();
                if (actor == null)
                {
                    Console.WriteLine("The actor does not exist");
                }
                else
                {
                    context.Remove(actor);
                    try
                    {
                        context.SaveChanges();

                    }
                    catch (DbUpdateException ex)
                    {
                        // logoj
                        // ktu vendoset ca do boj me errorin, do e rethrow, apo do e 'handle'
                        //throw;
                    }
                }
            }
        }

        static void DeleteMovie(int movieID)
        {
            using (var context = new DatabaseContext())
            {
                var movie = context.Movies
                    .Where(a => a.ID == movieID)
                    .Include(a => a.ActorsList)
                    .FirstOrDefault();
                if (movie == null)
                {
                    Console.WriteLine("The movie does not exist");
                }
                else
                {
                    context.Remove(movie);
                    SaveChangesWrapper(context);
                }
            }
        }

        static void SaveChangesWrapper(DbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void UpdateMovieName(int movieID)
        {
            using (var context = new DatabaseContext())
            {
                var movie = context.Movies
                    .Where(m => m.ID == movieID)
                    .FirstOrDefault();

                Console.WriteLine("Enter new movie title: ");
                string newTitle1 = Console.ReadLine();

                movie.Title = newTitle1;

                context.SaveChanges();
            }
        }

        static void UpdateMovieNameSP(int movieID)
        {
            using (var context = new DatabaseContext())
            {
                Console.WriteLine("Enter new movie title: ");
                string newTitle = Console.ReadLine();
                var param = new SqlParameter[]
                {
                 new SqlParameter()
                 {
                   ParameterName = "@ID",
                   SqlDbType =  System.Data.SqlDbType.Int,
                   Direction = System.Data.ParameterDirection.Input,
                   Value = movieID
                 },

                  new SqlParameter()
                  {
                    ParameterName = "@Title",
                    SqlDbType =  System.Data.SqlDbType.VarChar,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = newTitle
                  }
                };
                var setNewName = context.Movies.FromSqlRaw("[dbo].[UpdateMovieName]  @ID, @Title", param);
                context.SaveChanges();
            }
        }

        static void GetMovieByID(int movieID)
        {
            using (var context = new DatabaseContext())
            {
                var param1 = new SqlParameter[]
                {
                    new SqlParameter()
                    {
                       ParameterName = "@ID",
                       SqlDbType =  System.Data.SqlDbType.Int,
                       Direction = System.Data.ParameterDirection.Input,
                       Value = movieID
                    }
                };
                var getMovie = context.Movies.FromSqlRaw("[dbo].[GetMovieByID]  @ID", param1);
                
            }
        }
    }

}
















