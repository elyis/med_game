namespace med_game.src
{
    public static class Constants
    {
        //https://a1b9-95-183-24-214.ngrok-free.app
        //Environment.GetEnvironmentVariable("ASPNETCORE_URLS")!.Split(";").First()
        private static readonly string server_url = "https://a1b9-95-183-24-214.ngrok-free.app";
        

        //local photo storage 
        public static readonly string pathToIcons = @"src\Resources\Storages\";
        public static readonly string pathToAnswerIcons = $@"{pathToIcons}AnswerIcons\";
        public static readonly string pathToQuestionIcons = $@"{pathToIcons}QuestionIcons\";
        public static readonly string pathToAchievementIcons = $@"{pathToIcons}AchievementIcons\";
        public static readonly string pathToProfileIcons = $@"{pathToIcons}ProfileIcons\";

        //web path to photo
        public static readonly string webPathToAnswerIcons = $@"{server_url}/answerIcon/";
        public static readonly string webPathToAchievementIcons = $@"{server_url}/achievementIcon/";
        public static readonly string webPathToQuestionIcons = $@"{server_url}/questionIcon/";
        public static readonly string webPathToProfileIcons = $@"{server_url}/profileIcon/";
    }
}
