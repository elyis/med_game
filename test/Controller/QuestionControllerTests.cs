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
                            type = TypeAnswer.Text,
                            text = "Опорная, двигательная, защитная, биологическая",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Опорная, двигательная, защитная, биологическая",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Вместилище внутренних органов, депо минеральных веществ",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Кровеотводная, защитная, двигательная",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "Органические и неорганический вещества",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Органические и неорганический вещества",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Белки, жиры, углеводы, соли кальция",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Кальций, магний, фосфор, марганец, белки",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "Большеберцовая кость",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Большеберцовая кость",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Малоберцовая кость",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Бедренная кость",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Надколенник",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Таранная кость",
                            image = null,
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
                    Image = "skeleton.jpg",
                    RightAnswer = new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Плечевая кость",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Большеберцовая кость",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Малоберцовая кость",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Бедренная кость",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Плечевая кость",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Таранная кость",
                            image = null,
                        }
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
                    RightAnswer = new AnswerOption
                        {
                            type = TypeAnswer.Input,
                            text = "ушковидная поверхность крестца",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Input,
                            text = "ушковидная поверхность крестца",
                            image = null,
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
                        type = TypeAnswer.Text,
                        text = "Мезодерма сомитов",
                        image = null,
                    },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Эктодерма",
                            image = null
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Мезодерма сомитов",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Мезодерма спланхнотомов",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Энтодерма",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "Одноперистая",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Веретенообразная",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Одноперистая",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Двуперистая",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Многоперистая",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "Остистые отростки позвонков",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Поперечные отростки позвонков",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Остистые отростки позвонков",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Суставные отростки позвонков",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Шейки ребер",
                            image = null,
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
                    Image = "myology.jpeg",
                    RightAnswer = new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "pectoralis major",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "pectoralis major",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "psoas major",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "platysma",
                            image = null,
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
                            type = TypeAnswer.Image,
                            text = null,
                            image = "img_1.png",
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Image,
                            text = null,
                            image = "img_1.png",
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Image,
                            text = null,
                            image = "img_2.png",
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Image,
                            text = null,
                            image = "img_3.png",
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
                            type = TypeAnswer.Text,
                            text = "labia oris",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "labia oris",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "dens",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Lingua",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "6-7 лет",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "9-10 лет",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "6-7 лет",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "8 лет",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "желобовидные сосочки",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "нитевидные сосочки",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "грибовидные сосочки",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "желобовидные сосочки",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "20",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "24",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "20",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "16",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "Нейрон",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Синапс",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Нейрон",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Рецептор",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Рефлекс",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "шейное",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "терминальное",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "копчиковое",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "грудное",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "шейное",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "8",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "5",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "7",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "8",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "12",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "Центральная нервная система",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Центральная нервная система",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Периферическая нервная система",
                            image = null,
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
                            type = TypeAnswer.Text,
                            text = "Периферическая нервная система",
                            image = null,
                        },
                    ListOfAnswers = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Центральная нервная система",
                            image = null,
                        },

                        new AnswerOption
                        {
                            type = TypeAnswer.Text,
                            text = "Периферическая нервная система",
                            image = null,
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
