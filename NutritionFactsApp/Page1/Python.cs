using Python.Runtime;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.IO;

namespace NutritionFactsApp
{
    internal class Python
    {
        public async Task<string> RunGeneratorAsync(NutritionModel model, string outputPath = null)
        {
            string targetPath = outputPath ?? Path.Combine(Path.GetTempPath(), $"nutrition_{Guid.NewGuid().ToString().Substring(0, 8)}.png");

            await Task.Run(() =>
            {
                if (!PythonEngine.IsInitialized) return;

                using (Py.GIL())
                {
                    try
                    {
                        string json = JsonConvert.SerializeObject(model);
                        dynamic scope = Py.CreateScope();
                        scope.Set("data_json", json);
                        scope.Set("output_path", targetPath);

                        // sẽ thêm metadata đây
                        string script = $@"
import json
import sys
import os

sys.path.append(os.getcwd())
from nutrition_facts.models import NutritionData, ServingInfo, Nutrient
from nutrition_facts.generator import NutritionFactsGenerator

d = json.loads(data_json)

# Chuẩn FDA (DV) cho cả Đa lượng và Vi lượng
# Đơn vị trong dict: g (đa lượng), mg/mcg (vi lượng)
DV_STANDARDS = {{
    'Total Fat': 78, 'Saturated Fat': 20, 'Cholesterol': 300, 
    'Sodium': 2300, 'Total Carbohydrate': 275, 'Dietary Fiber': 28, 'Protein': 50,
    'Vitamin D': 20,    # mcg
    'Calcium': 1300,    # mg
    'Iron': 18,         # mg
    'Potassium': 4700   # mg
}}

def get_dv(name, value):
    if value <= 0: return 0
    return int(round((value / DV_STANDARDS.get(name, 1)) * 100))

nutrients = [
    Nutrient(name='Total Fat', value=int(d['TotalFat']), unit='g', daily_value_percent=get_dv('Total Fat', int(d['TotalFat']))),
    Nutrient(name='Saturated Fat', value=int(d['SaturatedFat']), unit='g', daily_value_percent=get_dv('Saturated Fat', int(d['SaturatedFat']))),
    Nutrient(name='Trans Fat', value=int(d['TransFat']), unit='g'),
    Nutrient(name='Cholesterol', value=int(d['Cholesterol']), unit='mg', daily_value_percent=get_dv('Cholesterol', int(d['Cholesterol']))),
    Nutrient(name='Sodium', value=int(d['Sodium']), unit='mg', daily_value_percent=get_dv('Sodium', int(d['Sodium']))),
    Nutrient(name='Total Carbohydrate', value=int(d['TotalCarbohydrate']), unit='g', daily_value_percent=get_dv('Total Carbohydrate', int(d['TotalCarbohydrate']))),
    Nutrient(name='Dietary Fiber', value=int(d['DietaryFiber']), unit='g', daily_value_percent=get_dv('Dietary Fiber', int(d['DietaryFiber']))),
    Nutrient(name='Total Sugars', value=int(d['TotalSugars']), unit='g'),
    Nutrient(name='Added Sugars', value=int(d['AddedSugars']), unit='g'),
    Nutrient(name='Protein', value=int(d['Protein']), unit='g', daily_value_percent=get_dv('Protein', int(d['Protein']))),
    
    # Vi lượng: Tự tính dựa trên giá trị mg/mcg nhập vào
    Nutrient(name='Vitamin D', value=int(d['VitaminD']), unit='mcg', daily_value_percent=get_dv('Vitamin D', int(d['VitaminD']))),
    Nutrient(name='Calcium', value=int(d['Calcium']), unit='mg', daily_value_percent=get_dv('Calcium', int(d['Calcium']))),
    Nutrient(name='Iron', value=int(d['Iron']), unit='mg', daily_value_percent=get_dv('Iron', int(d['Iron']))),
    Nutrient(name='Potassium', value=int(d['Potassium']), unit='mg', daily_value_percent=get_dv('Potassium', int(d['Potassium'])))
]


unit_text = str(d.get('ServingUnit', '')).strip()
weight_val = d.get('ServingWeight', 0)





#ghép chuỗi đơn vị và size 
if unit_text and unit_text != '{Constants.PlaceHolderForTxtServingUnit}':
    formatted_size = f'{{unit_text}} ({{int(weight_val)}}g)'
else:
    formatted_size = f'{{int(weight_val)}}g'








# Đảm bảo data nằm thẳng hàng
data = NutritionData(
    product_name=d.get('ProductName', 'Product'),
    serving_info=ServingInfo(serving_size=formatted_size, servings_per_container=int(d['ServingCount'])),
    calories=int(d['Calories']),
    nutrients=nutrients
)

gen = NutritionFactsGenerator()
gen.generate(data, output_path)
";
                        scope.Exec(script);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Lỗi Python: " + ex.Message);
                    }
                }
            });
            return targetPath; // Trả về đường dẫn file đã tạo
        }
    }
}