namespace UKParliamentEndPointsAIChat.Ui.Models
{
    public class Config
    {
        public bool UseAzureHostedLlm { get; set; }
        public string SystemPrompt { get; set; }
        public string CoachAndFocusLLMApiKey { get; set; }
        public string CoachAndFocusLLMEndpoint { get; set; }
        public string CoachAndFocusLLMApiKey2 { get; set; }
        public string CoachAndFocusLLMEndpoint2 { get; set; }
        public double AITemperature { get; set; }
        public int AIMaxTokens { get; set; }
        public bool AIUseStream { get; set; }
        public double AITop_p { get; set; }
    }
}
