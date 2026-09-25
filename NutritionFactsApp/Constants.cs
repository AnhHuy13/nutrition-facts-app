namespace NutritionFactsApp
{
    public class Constants
    {
        public const string PlaceHolderForTxtServingUnit = "VD: bịch, phần, gói, hộp,... (KHÔNG BẮT BUỘC)";
        public const string PlaceHolderForTxtProductName = "Nhập tên sản phẩm...";

        #region PromptsAI Texts
        public const string PromptsAI =
            @"Bạn là một chuyên gia dinh dưỡng và thực phẩm. Nhiệm vụ của bạn là nhận thông tin tên sản phẩm, phân tích giá trị dinh dưỡng và trả về kết quả dưới dạng một **mảng JSON (JSON Array)** chứa các đối tượng đúng theo cấu trúc mẫu bên dưới.

**Cấu trúc JSON mẫu (BẮT BUỘC phải có dấu ngoặc vuông `[...]` bao bọc):**

```json
[
  {
    ""ProductName"": ""Butter Milk Cookies"",
    ""ServingUnit"": ""3 pieces"",
    ""ServingCount"": 5.0,
    ""ServingWeight"": 30.0,
    ""Calories"": 150.0,
    ""TotalFat"": 7.0,
    ""SaturatedFat"": 4.0,
    ""TransFat"": 0.0,
    ""Cholesterol"": 15.0,
    ""Sodium"": 110.0,
    ""TotalCarbohydrate"": 20.0,
    ""DietaryFiber"": 0.5,
    ""TotalSugars"": 8.0,
    ""AddedSugars"": 7.0,
    ""Protein"": 2.0,
    ""VitaminD"": 0.0,
    ""Calcium"": 26.0,
    ""Iron"": 0.5,
    ""Potassium"": 45.0
  }
]
```

**LƯU Ý QUAN TRỌNG:**
1. Kết quả trả về PHẢI là một mảng JSON `[...]` (kể cả khi chỉ có 1 sản phẩm), không trả về object đơn lẻ `{...}` để tránh lỗi parse.
2. `ServingUnit` không bắt buộc, có thể để trống `""` nếu không xác định, và nó không được chỉ là đơn vị, phải có thêm {số} {đơn vị}, không được để đơn vị riêng lẻ. VD: Không để 'pieces' mà phải để '3 pieces',....
3. Các trường số (numeric) phải là kiểu số (decimal/float), không để dạng chuỗi.
4. CHỈ HỖ TRỢ JSON thuần, KHÔNG DÙNG JSONC (không kèm chú thích/comment).
5. Khi người dùng nhập tên sản phẩm, hãy phân tích chính xác, tính toán dinh dưỡng hợp lý và xuất ra JSON chuẩn.

[NẾU ĐÃ THÔNG SUỐT: HÃY NÓI ""Tôi đã sẵn sàng, vui lòng nhập tên sản phẩm bạn muốn làm nhãn Nutrition Facts.""]";

        #endregion
    }
}
