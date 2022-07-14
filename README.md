# CinemaHub
Website for all movie and tv things (recommendations, watchlist, reviews, discussion) made with ASP.NET Core for Softuni C# Web - Project Defense September 2020

## Azure Site Link
May load slowly the first time you enter, because Azure turns off the app after inactivity and then turns it on when someone sends request. 
https://thecinemahub.azurewebsites.net/


## Project Description
This web application is made to organize your entertainment media life. You can search and query from a large selection of movies and tv series, add them to watchlist where you can track which movies and tv shows you have watched, want to watch, dropped or are currently watching. Every media can be rated by users, and moreover you can review it so you tell people what you think of it. Also, you can create discussions with fellow users and talk about the media or make any remarks. Based on your ratings and watchlist you are recommended more media - the more you rate and add to watchlist, the better the recommendations get.
![alt-text](https://i.ibb.co/tbsZGWz/first-page.png)

## Search media
The page for Movies or TV Shows is where you search them by Title, Keywords and Genre and sort them by Release Date, Popularity or Rating. Alternatively for quick access you can use the search bar at top of the menu. 20000 movies and tv shows are seeded when you run the application for the first time from The Movie Database - https://www.themoviedb.org/


## Reviews and Ratings
Users can create reviews of movies seen by everyone. You can only create reviews if you rated the given media beforehand.


## Discussions
Users can create discussions on each media and comment each other's discussions.


## Recommendations
Recommendations are created on a hybrid basis between ML.NET implementation and database query of keywords and genres. Basically, users either get:
1. Content-based recommendations - based on keywords and genres which the user rated higher and recently.
2. Collaborative-based recommendations - based on ML.NET's matrix factorization. (if users rated high same things they are likely to do so in the future approach)
Problem with using ML.NET is that it needs a good dataset, which you dont have at the start, so this recommendation 'activates' after 500 ratings from users

![alt-text](https://i.ibb.co/GW6md0S/second-page.png)

## Administration
User can edit media but they first need approval from administrator. An administrator can also delete every user-owned resource. Also, he has access to the Hangbar Dashboard which give control over the background processes.


## Watchlist
Users can add media to their watchlist (Want to Watch, Currently Watching, Completed, Dropped) which helps them track their entertainment. Every added media won't show up on recommendations.


## Profile
The core identity functionality - if you register you get confirmation email by SendGrid. Profiles have avatar images which can be changed.

In conclusion this is a very small IMDB clone site.

## Used Technologies
- ASP.NET Core 3.1 MVC
- ASP.NET Core areas
- Entity Framework Core 3.1
- Newtonsoft.Json
- SendGrid
- Bootstrap
- Tagify.js (keywords for media)
- Hangfire .NET (seeding and training model)
- ML.NET
- AJAX
- jQuery
- JavaScript
- xUnit
- Rater.js (Ratings stars)
- Mock.Queryable (Testing ToListAsync(), CountAsync(), etc.)

 ## Used Methods
- POST-REDIRECT-GET Pattern
- Resource-based Authorization (users can manipulate only their content)
- Automapping from template

## Credits
Author - Simeon Stefanov
HTML/CSS Template - https://www.templateshub.net/
