using System.Linq;
using System.Windows.Forms;

namespace NutritionFactsApp
{
    public static class JsonMapper
    {
        public static void MapUIToModel(Control parent, NutritionModel model)
        {
            if (parent is UserControl1 Ui)
            {
                if (!string.IsNullOrEmpty(Ui.txtProductName.Text) && Ui.txtProductName.Text != Constants.PlaceHolderForTxtProductName)
                {
                    model.ProductName = Ui.txtProductName.Text;
                }
                else
                {
                    model.ProductName = "";
                }
                if (!string.IsNullOrEmpty(Ui.txtServingUnit.Text) && Ui.txtServingUnit.Text != Constants.PlaceHolderForTxtServingUnit)
                {
                    model.ServingUnit = Ui.txtServingUnit.Text;
                }
                else
                {
                    model.ServingUnit = "";
                }
                //model.Code = Ui.codeEditor.Text;
                model.ServingCount = Ui.nudServingCount.Value;
                model.ServingWeight = Ui.nudServingWeight.Value;
                model.Calories = Ui.nudCalories.Value;
                model.TotalFat = Ui.nudTotalFat.Value;
                model.SaturatedFat = Ui.nudSaturatedFat.Value;
                model.TransFat = Ui.nudTransFat.Value;
                model.Cholesterol = Ui.nudCholesterol.Value;
                model.Sodium = Ui.nudSodium.Value;
                model.TotalCarbohydrate = Ui.nudTotalCarbs.Value;
                model.DietaryFiber = Ui.nudDietaryFiber.Value;
                model.TotalSugars = Ui.nudTotalSugars.Value;
                model.AddedSugars = Ui.nudAddedSugars.Value;
                model.Protein = Ui.nudProtein.Value;
                model.VitaminD = Ui.nudVitaminD.Value;
                model.Calcium = Ui.nudCalcium.Value;
                model.Iron = Ui.nudIron.Value;
                model.Potassium = Ui.nudPotassium.Value;
            }
        }

        // Map từ 1 Model cụ thể ra UI
        public static void MapModelToUI(Control parent, NutritionModel model)
        {
            if (parent is UserControl1 Ui)
            {
                Ui.txtProductName.Text = model.ProductName;
                Ui.txtServingUnit.Text = model.ServingUnit;
                //Ui.codeEditor.Text = model.Code;
                Ui.nudServingCount.Value = model.ServingCount;
                Ui.nudServingWeight.Value = model.ServingWeight;
                Ui.nudCalories.Value = model.Calories;
                Ui.nudTotalFat.Value = model.TotalFat;
                Ui.nudSaturatedFat.Value = model.SaturatedFat;
                Ui.nudTransFat.Value = model.TransFat;
                Ui.nudCholesterol.Value = model.Cholesterol;
                Ui.nudSodium.Value = model.Sodium;
                Ui.nudTotalCarbs.Value = model.TotalCarbohydrate;
                Ui.nudDietaryFiber.Value = model.DietaryFiber;
                Ui.nudTotalSugars.Value = model.TotalSugars;
                Ui.nudAddedSugars.Value = model.AddedSugars;
                Ui.nudProtein.Value = model.Protein;
                Ui.nudVitaminD.Value = model.VitaminD;
                Ui.nudCalcium.Value = model.Calcium;
                Ui.nudIron.Value = model.Iron;
                Ui.nudPotassium.Value = model.Potassium;
            }
        }
    }
}