 # Newsers News Agency

This application is a news agency website with an administration panel, which allows different news correspondents to upload their specialist
articles from their dedicated account. A general administrator has access to all administration panel options and all news articles.

**Programming languages used**: C# ASP.NET Core, JavaScript/AJAX (including plugins and libraries) and JQuery.

**Markup used and styling**: HTML5, Bootstrap 5, CSS3, Font Awesome

**Database(s) used**: SQL Server (via SSMS - SQL Server Management Studio)

## Tables/Views used in SQL Server (with columns)

<ins>Native tables<ins>

1. Category
   1. Id (PK)
   2. Title
   3. Description
2. Comment
   1. Id (PK)
   2. FullName
   3. Email
   4. CommentText
   5. CreatedAt
   6. IsApproved
   7. NewsId (FK)
3. Menu
   1. Id (PK)
   2. Title
   3. Link
   4. ParentId (not null is sub-menu item)
   5. Priority (controls the order of the menu item(s))
4. Tag
   1. Id (PK)
   2. Title
5. User
   1. Id (PK)
   2. FullName
   3. Username
   4. Password
   5. IsActive
   6. IsAdmin
6. News
   1. Id (PK)
   2. Title
   3. ShortDescription
   4. LongDescription
   5. CreatedAt
   6. ViewCount
   7. Status (Published or Unpublished)
   8. ImageName
   9. CategoryId (FK)
   10. Tags
   11. UserId (FK)
7. Settings
   1. Id (PK)
   2. Title
   3. Address
   4. Email
   5. Phone
   6. Copyright
   7. Facebook
   8. X
   9. Instagram
   10. YouTube
   11. LinkedIn
   12. FeaturedNews
   13. MainNews
   14. TopStory
   15. BestNews
   16. MainPageCategories
8. Subscriber
   1. Id
   2. Email
   3. SubscribedAt
   4. IsActive

<ins>Derived Views</ins>

1. NewsView (Forms a LEFT OUTER JOIN using the News and User tables - allows allocation of correspondent specific news)
2. PopularCategories (Forms a LEFT OUTER JOIN between the Category and News tables - counts the news belonging to each category ans groups them by Title and Id and uses NewsCount as an alias)
3. PopularNews (This is based on the news articles with the most comments using a RIGHT OUTER JOIN of the News and Comment tables - includes all the columns from the News table but with an extra columns for the quantity of comments user each news article (the alias CommentCount is used for this derived column)


## News Agency Site Navigation

### Navbar Area:

