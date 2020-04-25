using AutoMapper;
using Microsoft.Azure.Cosmos.Table;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreFourServer.Adapters.Azure.TableEntities
{
    public class ClientTokenTableEntity : TableEntity
    {
        private static readonly IMapper mapper;

        static ClientTokenTableEntity()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ClientToken, ClientTokenTableEntity>()
                    .ForMember(dst => dst.PartitionKey, opt => opt.MapFrom(src => src.Token.ToString("D")))
                    .ForMember(dst => dst.RowKey, opt => opt.MapFrom(src => Guid.NewGuid().ToString("D")))
                    ;
            });
            mapper = configuration.CreateMapper();
        }

        public static explicit operator ClientTokenTableEntity(ClientToken obj)
        {
            return mapper.Map<ClientToken, ClientTokenTableEntity>(obj);
        }

        public static explicit operator ClientToken(ClientTokenTableEntity obj)
        {
            return new ClientToken(obj.GameUserId, obj.ClientId, obj.Token, obj.Timeout);
        }

        public Guid GameUserId { get; set;}
        public Guid ClientId { get;set;}
        public Guid Token { get; set;}
        public DateTimeOffset Timeout { get; set;}
    }
}
