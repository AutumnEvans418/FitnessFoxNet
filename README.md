# Fitness Fox
A local, crossplatform, and secure fitness app built in Blazor .NET 8 that supports iOS, Android, and Web.
<img width="372" height="764" alt="image" src="https://github.com/user-attachments/assets/0fd7fd00-2e9a-4c65-a409-1972c10ed06e" />

> This project is still a work in progress and likely contains many bugs.

## Why
After going through some important life events, I've started to focus more on my health and I wanted to start tracking my health data. After looking at various fitness apps out there, I decided that I would just start with storing my data in google sheets. At the time I didn't need anything complicated and didn't want to pay for a service. Additionally, I needed to track less common data than what most health apps typically allowed for. 

Eventually, I started to build an app to make inputting that data easier as well as creating a way to automatically syncronize the data in the app to my google spreadsheet, hence that's why this project exists.

## High Level Features
- Manage Foods, Recipes, Meals, Vitals, Measurements, and Excercises (WIP).
- Set various goals to track your progress.
- Automatically syncronize to google sheets.
- Light/Dark Mode :)
- Supports running on the web AND on mobile!
- Supports various device sizes.
- OCR for reading labels (WIP).

## Why the Name?
Because foxes are cute and fit.

## Getting Started
- Clone the project
- Run `dotnet build` to build the project
- Run `dotnet test` to run the unit tests

## Technology Used
- .NET 8
- Blazor
- MudBlazor
- Google Cloud API
- Google Sheets
- Entity Framework Core (Sqlite)
- OpenCV
- Tesseract

## To List
- [x] Create User Accounts (Mainly used when deploying to the web).
- [x] Manage Foods
- [x] Manage Recipes
- [x] Manage Meals
- [x] Manage Vitals
- [x] Manage Goals
- [x] Manage Water Consumption
- [x] Manage Figure Measurements
- [x] Vitals Reports
- [ ] Manage Figure Goals
- [ ] OCR Scan Food Labels
- [ ] Barcode Scan Foods
- [ ] See what you can eat based on food goals
- [ ] Manage Cardio/Strength Excercises
- [ ] Parse Recipes from sites
