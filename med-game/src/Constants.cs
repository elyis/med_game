﻿namespace med_game.src
{
    public class Constants
    {
        private const string server_url = "http://192.168.1.165:5121/";

        //local photo storage 
        public const string pathToIcons = @"src\Resources\Storages\";
        public const string pathToAnswerIcons = $@"{pathToIcons}AnswerIcons\";
        public const string pathToQuestionIcons = $@"{pathToIcons}QuestionIcons\";
        public const string pathToAchievementIcons = $@"{pathToIcons}AchievementIcons\";
        public const string pathToProfileIcons = $@"{pathToIcons}ProfileIcons\";

        //web path to photo
        public const string webPathToAnswerIcons = $@"{server_url}answerIcon/";
        public const string webPathToAchievementIcons = $@"{server_url}achievementIcon/";
        public const string webPathToQuestionIcons = $@"{server_url}questionIcon/";
        public const string webPathToProfileIcons = $@"{server_url}profileIcon/";
    }
}
