[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.Calendar/master/Shared/NuGet/Icon.png "Zebble.Calendar"


## Zebble.Calendar

![logo]

A Zebble plugin that allow you to show a calendar in Zebble applications.


[![NuGet](https://img.shields.io/nuget/v/Zebble.Calendar.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.Calendar/)

> With this plugin you can add a calendar into your application that user can select a date.

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.Calendar/](https://www.nuget.org/packages/Zebble.Calendar/)
* Install in your platform client projects.
* Available for iOS, Android and UWP.
<br>


### Api Usage

To add this plugin into your page you can use below code:

```xml
<Calendar MultiSelectable="true" ShowWeekdays="true"/>
```

### Properties
| Property     | Type         | Android | iOS | Windows |
| :----------- | :----------- | :------ | :-- | :------ |
| Scope            | CalendarScope           | x       | x   | x       |
| LimitSelectionToRange            | bool           | x       | x   | x       |
| MinDate            | DateTime?           | x       | x   | x       |
| MaxDate            | DateTime?           | x       | x   | x       |
| StartDate            | DateTime           | x       | x   | x       |
| StartDay            | DayOfWeek           | x       | x   | x       |
| MonthsToShow            | int           | x       | x   | x       |
| LockScope            | bool           | x       | x   | x       |
| MultiSelectable            | bool           | x       | x   | x       |
| SelectedDate            | DateTime           | x       | x   | x       |
| SelectedDates            | List<DateTime&gt;           | x       | x   | x       |
| ShowWeekdays            | bool           | x       | x   | x       |
| ShowInBetweenMonthLabels            | bool           | x       | x   | x       |
| ShowNumberOfWeeks            | bool           | x       | x   | x       |
| WeekdayFormat            | string           | x       | x   | x       |


### Methods
| Method       | Return Type  | Parameters                          | Android | iOS | Windows |
| :----------- | :----------- | :-----------                        | :------ | :-- | :------ |
| ShowYears         | Task| -| x       | x   | x       |
| ShowMonths         | Task| -| x       | x   | x       |
| ShowDays         | Task| -| x       | x   | x       |
| PrevMonthYearView         | Task| -| x       | x   | x       |
| NextMonthYearView         | Task| -| x       | x   | x       |
