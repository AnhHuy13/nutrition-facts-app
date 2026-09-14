namespace NutritionFactsApp
{
    public class NutritionModel
    {
        public string ProductName { get; set; } = "";
        public string ServingUnit { get; set; } = "";
        public decimal ServingCount { get; set; }
        public decimal ServingWeight { get; set; }
        public decimal Calories { get; set; }
        public decimal TotalFat { get; set; }
        public decimal SaturatedFat { get; set; }
        public decimal TransFat { get; set; }
        public decimal Cholesterol { get; set; }
        public decimal Sodium { get; set; }
        public decimal TotalCarbohydrate { get; set; }
        public decimal DietaryFiber { get; set; }
        public decimal TotalSugars { get; set; }
        public decimal AddedSugars { get; set; }
        public decimal Protein { get; set; }
        public decimal VitaminD { get; set; }
        public decimal Calcium { get; set; }
        public decimal Iron { get; set; }
        public decimal Potassium { get; set; }
    }
}