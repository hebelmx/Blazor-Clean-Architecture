using BlazorCleanArchitecture.Application.Articles.GetArticles;
using BlazorCleanArchitecture.Application.Articles.TogglePublishArticle;


namespace BlazorCleanArchitecture.Application.Articles
{
    public class ArticlesViewService : IArticlesViewService
    {
        private readonly IRequestBus _requestBus;
        public ArticlesViewService(IRequestBus requestBus)
        {
            _requestBus = requestBus;
        }
        public async Task<List<ArticleDto>?> GetArticlesByCurrentUserAsync()
        {
            var result = await _requestBus.Query(new GetArticlesByCurrentUserQuery());
            return result;
        }

        public async Task<ArticleDto?> TogglePublishArticleAsync(int articleId)
        {
            var result = await _requestBus.Send(new TogglePublishArticleCommand { ArticleId = articleId });
            return result;
        }
    }
}
