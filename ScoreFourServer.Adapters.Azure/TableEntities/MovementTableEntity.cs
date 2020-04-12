using AutoMapper;
using Microsoft.Azure.Cosmos.Table;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreFourServer.Adapters.Azure.TableEntities
{
    public class MovementTableEntity : TableEntity
    {
        private static readonly IMapper mapper;

        static MovementTableEntity()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Movement, MovementTableEntity>()
                    .ForMember(dst => dst.PartitionKey, opt => opt.MapFrom(src => src.GameRoomId.ToString("D")))
                    .ForMember(dst => dst.RowKey, opt => opt.MapFrom(src => src.Counter))
                ;
            });
            mapper = configuration.CreateMapper();
        }

        public static explicit operator MovementTableEntity(Movement obj)
        {
            return mapper.Map<Movement, MovementTableEntity>(obj);
        }

        public static explicit operator Movement(MovementTableEntity obj)
        {
            return new Movement(
                obj.X, obj.Y, obj.Counter, obj.PlayerId, obj.GameRoomId, obj.CreateDate);
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Counter { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameRoomId { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
