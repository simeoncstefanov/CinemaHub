namespace CinemaHub.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;
    using CinemaHub.Services.Mapping;
    using CinemaHub.Web.ViewModels;
    using CinemaHub.Web.ViewModels.Discussions;
    using CinemaHub.Web.ViewModels.Media;
    using MockQueryable.Core;
    using MockQueryable.Moq;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    public class DiscussionsServicesTests
    {
        private readonly IRepository<CommentVote> commentVoteRepo;
        private readonly IRepository<Media> mediaRepo;
        private readonly IDeletableEntityRepository<Comment> commentRepo;

        public DiscussionsServicesTests()
        {
            this.commentVoteRepo = new Mock<IRepository<CommentVote>>().Object;

            var mediaRepoMock = new Mock<IRepository<Media>>();
            mediaRepoMock.Setup(x => x.All()).Returns(this.GetMedias().AsQueryable());
            this.mediaRepo = mediaRepoMock.Object;

            this.commentRepo = new Mock<IDeletableEntityRepository<Comment>>().Object;

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
            AutoMapperConfig.RegisterMappings(Assembly.Load("CinemaHub.Services.Data.Tests"));
        }

        [Fact]
        public async Task GetDiscussionGetsCorrectData()
        {
            // Arrange
            var discussionList = this.GetDiscussions();
            var expectedDiscussion = discussionList[1];
            var expectedTitle = expectedDiscussion.Title;
            var expectedDiscussionMedia = expectedDiscussion.MediaId;

            var discussionRepoMock = this.GetDiscussionMock(this.GetDiscussions()).Object;

            var discussionService = new DiscussionsService(discussionRepoMock, this.commentRepo, this.commentVoteRepo, this.mediaRepo);

            // Act
            var discussionViewModel = await discussionService.GetDiscussions<DiscussionViewModel>(expectedDiscussionMedia, 1, 2);

            // Assert
            Assert.Single(discussionViewModel);
            Assert.Contains(discussionViewModel, x => x.Title == expectedTitle);
        }

        [Fact]
        public async Task GetCommentsGetsCorrectData()
        {
            // Arrange
            var commentsList = this.GetComments();
            var discussionId = "10";
            var expectedCommentsCount = commentsList.Where(x => x.DiscussionId == discussionId).Count();
            var expectedContent = commentsList.FirstOrDefault(x => x.DiscussionId == discussionId);


            var commentsRepoMock = this.GetCommentMock(commentsList).Object;
            var discussionRepoMock = this.GetDiscussionMock(this.GetDiscussions()).Object;
            var commentService = new DiscussionsService(discussionRepoMock, commentsRepoMock, this.commentVoteRepo, this.mediaRepo);

            // Act
            var commentContent = await commentService.GetComments<CommentViewModel>(discussionId, 1, 2);

            // Assert
            Assert.Contains(commentContent, x => x.Content == expectedContent.Content);
            Assert.Equal(expectedCommentsCount, commentContent.Count());
        }

        [Fact]
        public async Task CreateDiscussionAddsCorrectly()
        {
            // Arrange
            var commentsList = this.GetComments();
            var discussionList = this.GetDiscussions();

            var expectedComments = commentsList.Count() + 1;
            var expectedDiscussion = discussionList.Count() + 1;

            var mediaList = this.GetMedias();
            var media = mediaList.FirstOrDefault().Id;
            var mediaRepoMock = new Mock<IRepository<Media>>();
            var mock = mediaList.AsQueryable().BuildMock();
            mediaRepoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);

            var commentsRepoMock = this.GetCommentMock(commentsList).Object;
            var discussionRepoMock = this.GetDiscussionMock(discussionList).Object;
            var commentService = new DiscussionsService(discussionRepoMock, commentsRepoMock, this.commentVoteRepo, mediaRepoMock.Object);

            var inputModel = new DiscussionInputModel()
            {
                Content = "Hell yeah",
                MediaId = media,
                Title = "Yes",
            };

            // Act
            await commentService.CreateDiscussion(inputModel, "");

            // Assert
            Assert.Equal(expectedComments, commentsRepoMock.AllAsNoTracking().Count());
            Assert.Equal(expectedDiscussion, discussionRepoMock.AllAsNoTracking().Count());
            Assert.Contains(discussionRepoMock.AllAsNoTracking().ToList(), x => x.MediaId == inputModel.MediaId && x.Title == inputModel.Title);
            Assert.Contains(commentsRepoMock.AllAsNoTracking().ToList(), x => x.Content == inputModel.Content);
        }

        [Fact]
        public async Task CreateCommentAddsCorrectly()
        {
            // Arrange
            var commentsList = this.GetComments();
            var expectedComments = commentsList.Count() + 1;

            var discussionList = this.GetDiscussions();
            var discussionId = discussionList.FirstOrDefault().Id;

            var commentsRepoMock = this.GetCommentMock(commentsList).Object;
            var discussionRepoMock = this.GetDiscussionMock(discussionList).Object;
            var commentService = new DiscussionsService(discussionRepoMock, commentsRepoMock, this.commentVoteRepo, this.mediaRepo);

            var inputModel = new CommentInputModel()
            {
                Content = "Hell yeah123",
                DiscussionId = discussionId,
            };

            // Act
            await commentService.CreateComment(inputModel.Content, "", inputModel.DiscussionId);

            // Assert
            Assert.Equal(expectedComments, commentsRepoMock.AllAsNoTracking().Count());
            Assert.Contains(commentsRepoMock.AllAsNoTracking().Where(x => x.DiscussionId == discussionId).ToList(), x => x.Content == inputModel.Content);
        }

        [Fact]
        public async Task DeleteCommentRemovesCorrectly()
        {
            // Arrange
            var commentsList = this.GetComments();
            var discussionList = this.GetDiscussions();
            var expectedComments = commentsList.Count() - 1;

            var comment = commentsList[0];
            var expectedId = comment.Id;


            var commentsRepoMock = this.GetCommentMock(commentsList).Object;
            var discussionRepoMock = this.GetDiscussionMock(discussionList).Object;
            var commentService = new DiscussionsService(discussionRepoMock, commentsRepoMock, this.commentVoteRepo, this.mediaRepo);

            // Act
            await commentService.DeleteComment(expectedId);

            // Assert
            Assert.Equal(expectedComments, commentsRepoMock.AllAsNoTracking().Count());
            Assert.DoesNotContain(commentsRepoMock.AllAsNoTracking(), x => x.Id == expectedId);
        }

        [Fact]
        public async Task DeleteDiscussionRemoveCorrectly()
        {
            // Arrange
            var commentsList = this.GetComments();
            var discussionList = this.GetDiscussions();
            var expectedDiscussion = commentsList.Count() - 1;

            var discussion = discussionList[0];
            var expectedId = discussion.Id;


            var commentsRepoMock = this.GetCommentMock(commentsList).Object;
            var discussionRepoMock = this.GetDiscussionMock(discussionList).Object;
            var commentService = new DiscussionsService(discussionRepoMock, commentsRepoMock, this.commentVoteRepo, this.mediaRepo);

            // Act
            await commentService.DeleteDiscussion(expectedId);

            // Assert
            Assert.Equal(expectedDiscussion, discussionRepoMock.AllAsNoTracking().Count());
            Assert.DoesNotContain(discussionRepoMock.AllAsNoTracking(), x => x.Id == expectedId);
        }

        [Fact]
        public async Task GetTotalDiscussionsGetsProperCount()
        {
            // Arrange
            var discussionList = this.GetDiscussions();
            var expectedMediaId = "2";
            var expectedCount = discussionList.Count(x => x.MediaId == expectedMediaId);

            var discussionRepoMock = this.GetDiscussionMock(this.GetDiscussions()).Object;
            var discussionService = new DiscussionsService(discussionRepoMock, this.commentRepo, this.commentVoteRepo, this.mediaRepo);

            // Act
            var discussionCount = await discussionService.GetTotalDiscussions(expectedMediaId);

            // Assert
            Assert.Equal(discussionCount, expectedCount);
        }

        [Fact]
        public async Task GetTotalCommentsGetProperCount()
        {
            // Arrange
            var commentsList = this.GetComments();
            var expectedDiscussionId = "10";
            var expectedCount = commentsList.Count(x => x.DiscussionId == expectedDiscussionId);

            var commentsRepoMock = this.GetCommentMock(commentsList).Object;
            var discussionRepoMock = this.GetDiscussionMock(this.GetDiscussions()).Object;
            var commentService = new DiscussionsService(discussionRepoMock, commentsRepoMock, this.commentVoteRepo, this.mediaRepo);

            // Act
            var commentsCount = await commentService.GetTotalComments(expectedDiscussionId);

            // Assert
            Assert.Equal(expectedCount, commentsCount);
        }

        [Fact]
        public async Task GetDiscussionTitleGivesCorrectTitle()
        {
            // Arrange
            var discussionList = this.GetDiscussions();
            var commentsList = this.GetComments();
            var expectedDiscussion = discussionList[1];
            var expectedDiscussionId = expectedDiscussion.Id;
            var expectedDiscussionTitle = expectedDiscussion.Title;

            var discussionRepoMock = this.GetDiscussionMock(discussionList).Object;
            var commentsRepoMock = this.GetCommentMock(commentsList).Object;
            var discussionService = new DiscussionsService(discussionRepoMock, commentsRepoMock, this.commentVoteRepo, this.mediaRepo);

            // Act
            var discussionTitle = await discussionService.GetDiscussionTitle(expectedDiscussionId);

            // Assert
            Assert.Equal(expectedDiscussionTitle, discussionTitle);
        }

        [Fact]
        public async Task GetDiscussionInfoGetsCorrectDetails()
        {
            // Arrange
            var discussionList = this.GetDiscussions();
            var expectedDiscussion = discussionList[1];
            var expectedDiscussionId = expectedDiscussion.Id;
            var expectedDiscussionTitle = expectedDiscussion.Title;

            var discussionRepoMock = this.GetDiscussionMock(discussionList).Object;
            var discussionService = new DiscussionsService(discussionRepoMock, this.commentRepo, this.commentVoteRepo, this.mediaRepo);

            // Act
            var discussion = await discussionService.GetDiscussionInfo<DiscussionViewModel>(expectedDiscussionId);
            var discussionTitle = discussion.Title;

            // Assert
            Assert.Equal(expectedDiscussionTitle, discussionTitle);
        }

        [Fact]
        public async Task GetCommentInfoGetsCorrectDetails()
        {
            // Arrange
            var commentsList = this.GetComments();
            var expectedComment = commentsList[1];
            var expectedCommentId = expectedComment.Id;
            var expectedCommentContent = expectedComment.Content;

            var commentsRepoMock = this.GetCommentMock(commentsList).Object;
            var discussionRepoMock = this.GetDiscussionMock(this.GetDiscussions()).Object;
            var commentService = new DiscussionsService(discussionRepoMock, commentsRepoMock, this.commentVoteRepo, this.mediaRepo);

            // Act
            var commentsInfo = await commentService.GetCommentInfo<CommentViewModel>(expectedCommentId);

            // Assert
            Assert.Equal(expectedCommentContent, commentsInfo.Content);
        }

        private List<Media> GetMedias()
        {
            var mediaList = new List<Media>()
            {
                new Show() { Title = "24", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now,
                           Images = new List<MediaImage>() { new MediaImage { Path = "", ImageType = ImageType.Poster } },
                           Ratings = new List<Rating>() { new Rating() { Score = 10 } }, },
                new Movie() { Title = "Inception", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now,
                           Images = new List<MediaImage>() { new MediaImage { Path = "", ImageType = ImageType.Poster } },
                           Ratings = new List<Rating>() { new Rating() { Score = 10 } }, },
                new Show() { Title = "Borat TV Series", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now,
                           Images = new List<MediaImage>() { new MediaImage { Path = "", ImageType = ImageType.Poster } },
                           Ratings = new List<Rating>() { new Rating() { Score = 10 } }, },
                new Movie() { Title = "Tenet", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now,
                           Images = new List<MediaImage>() { new MediaImage { Path = "", ImageType = ImageType.Poster } },
                           Ratings = new List<Rating>() { new Rating() { Score = 10 } }, },
            };

            foreach (var media in mediaList)
            {
                media.Genres = new List<MediaGenre>() { new MediaGenre { Genre = new Genre() { Id = 1, Name = "Action", ApiId = 1, }, Media = media } };
                media.Keywords = new List<MediaKeyword>();
            }

            return mediaList;
        }

        private Mock<IDeletableEntityRepository<Discussion>> GetDiscussionMock(List<Discussion> discussions)
        {
            var discussionRepoMock = new Mock<IDeletableEntityRepository<Discussion>>();
            var discussionsMock = discussions.AsQueryable().BuildMock();
            discussionRepoMock.Setup(x => x.AllAsNoTracking()).Returns(discussionsMock.Object);
            discussionRepoMock.Setup(x => x.All()).Returns(discussionsMock.Object);
            discussionRepoMock.Setup(x => x.Delete(It.IsAny<Discussion>())).Callback((Discussion disc) => discussions.Remove(disc));
            discussionRepoMock.Setup(x => x.AddAsync(It.IsAny<Discussion>())).Callback((Discussion disc) => discussions.Add(disc));

            return discussionRepoMock;
        }

        private Mock<IDeletableEntityRepository<Comment>> GetCommentMock(List<Comment> comments)
        {
            var repoMock = new Mock<IDeletableEntityRepository<Comment>>();
            var mock = comments.AsQueryable().BuildMock();
            repoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);
            repoMock.Setup(x => x.All()).Returns(mock.Object);
            repoMock.Setup(x => x.Delete(It.IsAny<Comment>())).Callback((Comment comment) => comments.Remove(comment));
            repoMock.Setup(x => x.AddAsync(It.IsAny<Comment>())).Callback((Comment comment) => comments.Add(comment));

            return repoMock;
        }

        private List<Discussion> GetDiscussions()
        {
            var discussionList = new List<Discussion>()
            {
                new Discussion()
                {
                    Title = "Its not that good of a movie",
                    Comments = new List<Comment>(),
                    Media = new Movie(),
                    MediaId = "1",
                    Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    IsDeleted = false,
                },
                new Discussion()
                {
                    Title = "Its that good of a movie",
                    Comments = new List<Comment>()
                    {
                        new Comment()
                        {
                            Content = "Yes, I agree",
                            Creator = new ApplicationUser()
                            {
                                UserName = "cryptosbg",
                                AvatarImage = new AvatarImage()
                                {
                                    Path = "",
                                },
                            },
                        },
                        new Comment()
                        {
                            Content = "Yesss, I agree",
                            Creator = new ApplicationUser()
                            {
                                UserName = "cryptosbg",
                                AvatarImage = new AvatarImage()
                                {
                                    Path = "",
                                },
                            },
                        },
                    },
                    Media = new Movie(),
                    MediaId = "2",
                    Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    IsDeleted = false,
                },
                new Discussion()
                {
                    Title = "Its that good of a movie for real",
                    Comments = new List<Comment>(),
                    Media = new Movie(),
                    MediaId = "1",
                    Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    IsDeleted = false,
                },
            };

            return discussionList;
        }

        public List<Comment> GetComments()
        {
            var commentsList = new List<Comment>()
            {
                new Comment()
                {
                    Content = "Yes, I agree",
                    Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    CommentVotes = new List<CommentVote>(),
                    DiscussionId = "9",
                    Discussion = new Discussion(),
                },
                new Comment()
                {
                    Content = "No, I disagree",
                    Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    CommentVotes = new List<CommentVote>(),
                    DiscussionId = "10",
                    Discussion = new Discussion(),
                },
                new Comment()
                {
                    Content = "Idk",
                    Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    CommentVotes = new List<CommentVote>(),
                    DiscussionId = "10",
                    Discussion = new Discussion(),
                },
            };

            return commentsList;
        }
    }
}
