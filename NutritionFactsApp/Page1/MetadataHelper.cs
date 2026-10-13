using Newtonsoft.Json;
using System;
using System.IO;

namespace NutritionFactsApp
{
    public static class MetadataHelper
    {
        public static string GetMetadataFilePath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return null;
            return Path.ChangeExtension(imagePath, ".json");
        }

        public static void SaveMetadata(string imagePath, NutritionModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath) || model == null) return;
                string metaPath = GetMetadataFilePath(imagePath);
                string json = JsonConvert.SerializeObject(model, Formatting.Indented);
                File.WriteAllText(metaPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi khi lưu metadata: " + ex.Message);
            }
        }

        public static NutritionModel ReadMetadata(string pathfile)
        {
            try
            {
                if (string.IsNullOrEmpty(pathfile)) return null;

                string metaPath = GetMetadataFilePath(pathfile);
                if (File.Exists(metaPath))
                {
                    string json = File.ReadAllText(metaPath);
                    return JsonConvert.DeserializeObject<NutritionModel>(json);
                }

                string altMetaPath = pathfile + ".json";
                if (File.Exists(altMetaPath))
                {
                    string json = File.ReadAllText(altMetaPath);
                    return JsonConvert.DeserializeObject<NutritionModel>(json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi khi đọc metadata: " + ex.Message);
            }
            return null;
        }
    }
}
