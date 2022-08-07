# FoodDatabase

```mermaid
erDiagram
    FoundationFoods ||--o{ FoodCategories : has
    FoundationFoods }o--|| FoodNutrients : contains
    FoodNutrients ||--o{ Nutrients : has
    FoundationFoods }o--|| FoodPortions : has
    FoodPortions ||--o{ MeasureUnits : has 
```