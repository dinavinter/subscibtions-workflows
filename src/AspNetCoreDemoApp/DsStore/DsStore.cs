using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreDemoApp.GigyaApi;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.InMemory;
using Elsa.Persistence.Specifications;

namespace AspNetCoreDemoApp.DsStore
{
    public class DsStore<T> : IStore<T> where T : IEntity
    {
        private readonly HttpJsonClient _httpJsonClient;
        private readonly Namespace _ds = new Api(GigyaSite.Test, GigyaCreds.Test).ds;
        private readonly string type = typeof(T).Name.ToLower();

        public DsStore(HttpJsonClient httpJsonClient)
        {
            _httpJsonClient = httpJsonClient;
        }

        public async Task SaveAsync(T entity, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _httpJsonClient.Send(_ds.GetRequest(new DsStoreRequest(entity.Id, type, entity)),
                cancellationToken);
            EnsureSuccessResponse(response);
        }


        public Task AddAsync(T entity, CancellationToken cancellationToken = new CancellationToken())
        {
            return SaveAsync(entity, cancellationToken);
        }

        public async Task AddManyAsync(IEnumerable<T> entities,
            CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entity in entities)
            {
                await SaveAsync(entity, cancellationToken);
            }
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = new CancellationToken())
        {
            return SaveAsync(entity, cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _httpJsonClient.Send(_ds.GetRequest(new DsDeleteRequest(entity.Id, type)),
                cancellationToken);
            EnsureSuccessResponse(response);
        }

        public async Task<int> DeleteManyAsync(ISpecification<T> specification,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var entities = await FindManyAsync(specification, cancellationToken: cancellationToken);
            var enumerable = entities as T[] ?? entities.ToArray();
            foreach (var entity in enumerable)
            {
                await DeleteAsync(entity, cancellationToken);
            }

            return enumerable.Count();
        }

        public async Task<IEnumerable<T>> FindManyAsync(ISpecification<T> specification, IOrderBy<T>? orderBy = null,
            IPaging? paging = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var values = await FindAll(cancellationToken);
            var query =values.AsQueryable().Apply(specification).Apply(orderBy).Apply(paging);
            return query.ToList();
        }

        public async Task<int> CountAsync(ISpecification<T> specification,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _httpJsonClient.Get<SearchResponse>(
                _ds.GetRequest(new DsSearchRequest($"select count(*) from {type}")), cancellationToken);
            EnsureSuccessResponse(response);
            return (int) response.SelectMetric("count(*)");
        }

        public async Task<T?> FindAsync(ISpecification<T> specification,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var values = await FindAll(cancellationToken);
            return values.AsQueryable().Apply(specification).FirstOrDefault();
        }

        private async Task<IEnumerable<T>> FindAll(CancellationToken cancellationToken)
        {
            var response =
                await _httpJsonClient.Get<SearchResponse>(
                    _ds.GetRequest(new DsSearchRequest($"select * from {type}")), cancellationToken);
            EnsureSuccessResponse(response);
            var values = response.SelectResults<T>();
            return values;
        }

        private static void EnsureSuccessResponse(GStatus response)
        {
            if (response.errorCode > 0)
            {
                throw new Exception($"Error {response.errorCode} {response.errorMessage} {response.errorDetails}  ");
            }
        }
    }
}