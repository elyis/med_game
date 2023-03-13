using med_game.src.Controllers;
using med_game.src.Entities.Request;
using med_game.src.Entities;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace test.Controller
{
    public class QuestionControllerTests
    {
        [Fact]
        public async Task SuccesfullyCreateQuestion()
        {
            QuestionController controller = new QuestionController();
            string lecternName = "Анатомия";
            List<StatusCodeResult> responces = new List<StatusCodeResult>();

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
                    RightAnswer = new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Опорная, двигательная, защитная, биологическая",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Опорная, двигательная, защитная, биологическая",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Вместилище внутренних органов, депо минеральных веществ",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Кровеотводная, защитная, двигательная",
                            Image = null,
                        }
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
                    RightAnswer = new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Органические и неорганический вещества",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Органические и неорганический вещества",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Белки, жиры, углеводы, соли кальция",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Кальций, магний, фосфор, марганец, белки",
                            Image = null,
                        }
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
                    RightAnswer = new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Большеберцовая кость",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Большеберцовая кость",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Малоберцовая кость",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Бедренная кость",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Надколенник",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Таранная кость",
                            Image = null,
                        }
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Остеология",
                    TypeQuestion = TypeQuestion.Image,
                    TimeSeconds = 78,
                    Text = "Что изображено под 6-ым пунктом?",
                    Description = null,
                    Image = "скелет.jpg",
                    RightAnswer = new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Плечевая кость",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Большеберцовая кость",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Малоберцовая кость",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Бедренная кость",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Плечевая кость",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Таранная кость",
                            Image = null,
                        }
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Остеология",
                    TypeQuestion = TypeQuestion.Input,
                    TimeSeconds = 78,
                    Text = "facies auricularis ossis sacri",
                    Description = null,
                    Image = null,
                    RightAnswer = new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "ушковидная поверхность крестца",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "ушковидная поверхность крестца",
                            Image = null,
                        }
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
                    RightAnswer = new AnswerOption
                    {
                        Type = TypeAnswer.Text,
                        Text = "Мезодерма сомитов",
                        Image = null,
                    },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Эктодерма",
                            Image = null
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Мезодерма сомитов",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Мезодерма спланхнотомов",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Энтодерма",
                            Image = null,
                        }
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
                        {
                            Type = TypeAnswer.Text,
                            Text = "Одноперистая",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Веретенообразная",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Одноперистая",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Двуперистая",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Многоперистая",
                            Image = null,
                        }
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
                        {
                            Type = TypeAnswer.Text,
                            Text = "Остистые отростки позвонков",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Поперечные отростки позвонков",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Остистые отростки позвонков",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Суставные отростки позвонков",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Шейки ребер",
                            Image = null,
                        }
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Миология",
                    TypeQuestion = TypeQuestion.Image,
                    TimeSeconds = 100,
                    Text = "Что указано под 4?",
                    Description = null,
                    Image = "миология.jpeg",
                    RightAnswer = new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "pectoralis major",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "pectoralis major",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "psoas major",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "platysma",
                            Image = null,
                        },
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
                    RightAnswer = new AnswerOption
                        {
                            Type = TypeAnswer.Image,
                            Text = null,
                            Image = "img_1.png",
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Image,
                            Text = null,
                            Image = "img_1.png",
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Image,
                            Text = "img_2.png",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Image,
                            Text = "img_3.png",
                            Image = null,
                        },
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
                        {
                            Type = TypeAnswer.Text,
                            Text = "labia oris",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "labia oris",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "dens",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Lingua",
                            Image = null,
                        },
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
                        {
                            Type = TypeAnswer.Text,
                            Text = "6-7 лет",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "9-10 лет",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "6-7 лет",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "8 лет",
                            Image = null,
                        },
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
                        {
                            Type = TypeAnswer.Text,
                            Text = "желобовидные сосочки",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "нитевидные сосочки",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "грибовидные сосочки",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "желобовидные сосочки",
                            Image = null,
                        },
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
                        {
                            Type = TypeAnswer.Text,
                            Text = "20",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "24",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "20",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "16",
                            Image = null,
                        },
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
                        {
                            Type = TypeAnswer.Text,
                            Text = "Нейрон",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Синапс",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Нейрон",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Рецептор",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Рефлекс",
                            Image = null,
                        }
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
                        {
                            Type = TypeAnswer.Text,
                            Text = "шейное",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "терминальное",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "копчиковое",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "грудное",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "шейное",
                            Image = null,
                        }
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
                        {
                            Type = TypeAnswer.Text,
                            Text = "8",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "5",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "7",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "8",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "12",
                            Image = null,
                        }
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Центральная нервная система",
                    TypeQuestion = TypeQuestion.Image,
                    TimeSeconds = 56,
                    Text = "Какая представлена система?",
                    Description = null,
                    Image = "img_1.png",
                    RightAnswer = new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Центральная нервная система",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Центральная нервная система",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Периферическая нервная система",
                            Image = null,
                        },
                    }
                },
                new RequestedQuestionBody
                {
                    LecternName = lecternName,
                    ModuleName = "Периферическая нервная система",
                    TypeQuestion = TypeQuestion.Image,
                    TimeSeconds = 56,
                    Text = "Какая представлена система?",
                    Description = null,
                    Image = "img_2.png",
                    RightAnswer = new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Периферическая нервная система",
                            Image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Центральная нервная система",
                            Image = null,
                        },

                        new AnswerOption
                        {
                            Type = TypeAnswer.Text,
                            Text = "Периферическая нервная система",
                            Image = null,
                        },
                    }
                },
            };

            foreach(var body in bodies)
            {
                var result = await controller.CreateQuestion(body);
                responces.Add(result as StatusCodeResult);
            }

            foreach(var response in responces)
            {
                Assert.Contains(response.StatusCode, validHttpCodes);
            }

            
        }

    }
}
