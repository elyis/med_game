using med_game.src.Controllers;
using med_game.src.Entities.Request;
using med_game.src.Entities;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Logging;
using med_game.src.Data;
using Microsoft.EntityFrameworkCore;

namespace test.Controller
{
    public class QuestionControllerTests
    {
        [Fact]
        public async Task SuccesfullyCreateQuestion()
        {
            QuestionController controller = new QuestionController(LoggerFactory.Create(config => config.AddConsole()), new AppDbContext(new DbContextOptions<AppDbContext>()));
            string lecternName = "Anatomy";
            List<StatusCodeResult> responces = new ();

            int[] validHttpCodes = new int[]
            {
                (int) HttpStatusCode.OK,
                (int) HttpStatusCode.Conflict
            };

            List<RequestedQuestionBody> bodies = new List<RequestedQuestionBody>
            {
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Остеология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 100,
                    Text = "Определите значение и функции скелета",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption(TypeAnswer.Text, "Опорная, двигательная, защитная, биологическая", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption(TypeAnswer.Text, "Опорная, двигательная, защитная, биологическая", null),
                        new AnswerOption(TypeAnswer.Text, "Вместилище внутренних органов, депо минеральных веществ", null),
                        new AnswerOption(TypeAnswer.Text, "Кровеотводная, защитная, двигательная", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Остеология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 100,
                    Text = "Назовите химический состав кости",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption(TypeAnswer.Text, "Органические и неорганический вещества", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption(TypeAnswer.Text, "Органические и неорганический вещества", null),
                        new AnswerOption(TypeAnswer.Text, "Белки, жиры, углеводы, соли кальция", null),
                        new AnswerOption(TypeAnswer.Text, "Кальций, магний, фосфор, марганец, белки", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Остеология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 78,
                    Text = "После cтолкновения двух автомобилей у одного из водителей отмечается деформация в средней трети левой голени, " +
                    "сильная боль, в особенности при попытке двигать левой голенью. Из раны выступают концы кости трехгранного сечения, " +
                    "усиливается кровопотеря. Какая кость может быть повреждена?",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption(TypeAnswer.Text, "Большеберцовая кость", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption(TypeAnswer.Text, "Большеберцовая кость", null),
                        new AnswerOption(TypeAnswer.Text, "Малоберцовая кость", null),
                        new AnswerOption(TypeAnswer.Text, "Бедренная кость", null),
                        new AnswerOption(TypeAnswer.Text, "Надколенник", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Остеология",
                    TypeQuestion = TypeQuestion.Image,
                    TimeSeconds = 78,
                    Text = null,
                    Description = "Что изображено под 6-ым пунктом?",
                    Image = "skeleton.jpg",
                    RightAnswer = new AnswerOption(TypeAnswer.Text, "Плечевая кость", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption(TypeAnswer.Text, "Большеберцовая кость", null),
                        new AnswerOption(TypeAnswer.Text, "Малоберцовая кость", null),
                        new AnswerOption(TypeAnswer.Text, "Бедренная кость", null),
                        new AnswerOption(TypeAnswer.Text, "Плечевая кость", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Остеология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 78,
                    Text = "facies auricularis ossis sacri",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption(TypeAnswer.Input, "ушковидная поверхность крестца", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption(TypeAnswer.Input, "ушковидная поверхность крестца", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Миология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 78,
                    Text = "Укажите источник развития скелетных мышц",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption(TypeAnswer.Text, "Мезодерма сомитов", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "Эктодерма", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Мезодерма сомитов", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Мезодерма спланхнотомов", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Энтодерма", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Миология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 100,
                    Text = "Укажите, как называется мышца, если мышечные пучки лежат по одну сторону от сухожилия",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "Одноперистая", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "Веретенообразная", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Одноперистая", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Двуперистая", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Многоперистая", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Миология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 100,
                    Text = "Укажите место начала трапециевидной мышцы",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "Остистые отростки позвонков", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "Поперечные отростки позвонков", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Остистые отростки позвонков", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Суставные отростки позвонков", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Шейки ребер", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Миология",
                    TypeQuestion = TypeQuestion.Image,
                    TimeSeconds = 100,
                    Text = null,
                    Description = "Что указано под 4?",
                    Image = "myology.jpeg",
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "pectoralis major", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "pectoralis major", null),

                        new AnswerOption
                        (TypeAnswer.Text, "psoas major", null),

                        new AnswerOption
                        (TypeAnswer.Text, "platysma", null),
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Миология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 32,
                    Text = "Укажите бедренную кость",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption(TypeAnswer.Image, null, "img_1.png"),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption(TypeAnswer.Image, null, "img_1.png"),
                        new AnswerOption
                        (TypeAnswer.Image, null, "img_2.png"),

                        new AnswerOption
                        (TypeAnswer.Image, null, "img_3.png"),
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Спланхнология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 32,
                    Text = "Предверье рта ограничивают",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "labia oris", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "labia oris", null),

                        new AnswerOption
                        (TypeAnswer.Text, "dens", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Lingua", null),
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Спланхнология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 56,
                    Text = "Первые постоянные зубы прорезаются",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "6-7 лет", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "9-10 лет", null),

                        new AnswerOption
                        (TypeAnswer.Text, "6-7 лет", null),

                        new AnswerOption
                        (TypeAnswer.Text, "8 лет", null),
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Спланхнология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 56,
                    Text = "Вкусовые сосочки на границе тела и корня языка",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "желобовидные сосочки", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "нитевидные сосочки", null),

                        new AnswerOption
                        (TypeAnswer.Text, "грибовидные сосочки", null),

                        new AnswerOption
                        (TypeAnswer.Text, "желобовидные сосочки", null),
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Спланхнология",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 56,
                    Text = "Количество молочных зубов у 3-ех летнего?",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "20", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "24", null),

                        new AnswerOption
                        (TypeAnswer.Text, "20", null),

                        new AnswerOption
                        (TypeAnswer.Text, "16", null),
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Центральная нервная система",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 56,
                    Text = "Структурно-функциональная единица нервной системы",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "Нейрон", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "Синапс", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Нейрон", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Рецептор", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Рефлекс", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Центральная нервная система",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 56,
                    Text = "Утолщение спинного мозга",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "шейное", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "терминальное", null),

                        new AnswerOption
                        (TypeAnswer.Text, "копчиковое", null),

                        new AnswerOption
                        (TypeAnswer.Text, "грудное", null),

                        new AnswerOption
                        (TypeAnswer.Text, "шейное", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Центральная нервная система",
                    TypeQuestion = TypeQuestion.Text,
                    TimeSeconds = 56,
                    Text = "Количество сегментов в шейном отделе спинного мозга",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "8", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "5", null),

                        new AnswerOption
                        (TypeAnswer.Text, "7", null),

                        new AnswerOption
                        (TypeAnswer.Text, "8", null),

                        new AnswerOption
                        (TypeAnswer.Text, "12", null)
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Центральная нервная система",
                    TypeQuestion = TypeQuestion.Image,
                    TimeSeconds = 56,
                    Text = null,
                    Description = "Какая представлена система?",
                    Image = "img_1.png",
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "Центральная нервная система", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "Центральная нервная система", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Периферическая нервная система", null),
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Периферическая нервная система",
                    TypeQuestion = TypeQuestion.Image,
                    TimeSeconds = 56,
                    Text = null,
                    Description = "Какая представлена система?",
                    Image = "img_2.png",
                    RightAnswer = new AnswerOption
                        (TypeAnswer.Text, "Периферическая нервная система", null),
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        (TypeAnswer.Text, "Центральная нервная система", null),

                        new AnswerOption
                        (TypeAnswer.Text, "Периферическая нервная система", null),
                    }
                },
            };

            foreach(var body in bodies)
            {
                var result = await controller.CreateQuestion(body);
                var code = result as StatusCodeResult;
                responces.Add(code);
            }

            foreach(var response in responces)
            {
                Assert.Contains((int)response.StatusCode, validHttpCodes);
            }

            
        }

    }
}
