using AutoMapper;
using Microsoft.Azure.Cosmos.Table;
using ScoreFourServer.Domain;
using ScoreFourServer.Domain.Entities;
using ScoreFourServer.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreFourServer.Adapters.Azure.TableEntities
{
    public class GameRoomTableEntity : TableEntity
    {
        private static readonly IMapper mapper;

        static GameRoomTableEntity()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GameRoom, GameRoomTableEntity>()
                    .ForMember(dst => dst.Player1GameUserId, opt => opt.MapFrom(src => src.Players[0].GameUserId))
                    .ForMember(dst => dst.Player1ClientId, opt => opt.MapFrom(src => src.Players[0].ClientId))
                    .ForMember(dst => dst.Player1Name, opt => opt.MapFrom(src => src.Players[0].Name))
                    .ForMember(dst => dst.Player2GameUserId, opt => opt.MapFrom(src => src.Players[1].GameUserId))
                    .ForMember(dst => dst.Player2ClientId, opt => opt.MapFrom(src => src.Players[1].ClientId))
                    .ForMember(dst => dst.Player2Name, opt => opt.MapFrom(src => src.Players[1].Name))
                    .ForMember(dst => dst.PartitionKey, opt => opt.MapFrom(src => src.CreateDate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")))
                    .ForMember(dst => dst.RowKey, opt => opt.MapFrom(src => src.GameRoomId.ToString("D")))
                    .ForMember(dst => dst.GameRoomStatus, opt => opt.MapFrom(src => src.GameRoomStatus.ToString()))
                    ;
                cfg.CreateMap<GameRoomTableEntity, GameRoom>()
                    .ForMember(dst => dst.Players, opt => opt.MapFrom(src => new List<Client>
                    {
                        new Client(src.Player1GameUserId, src.Player1ClientId,src.Player1Name),
                        new Client(src.Player2GameUserId, src.Player2ClientId, src.Player2Name),
                    }))
                    .ForMember(dst => dst.GameRoomStatus, opt => opt.MapFrom(src => Enum.Parse<GameRoomStatus>(src.GameRoomStatus)))
                    ;
            });
            mapper = configuration.CreateMapper();
        }

        public static explicit operator GameRoomTableEntity(GameRoom obj)
        {
            return mapper.Map<GameRoom, GameRoomTableEntity>(obj);
        }

        public static explicit operator GameRoom(GameRoomTableEntity obj)
        {
            return mapper.Map<GameRoomTableEntity, GameRoom>(obj);
        }

        public Guid GameRoomId { get; set; }
        public string Name { get; set; }
        public Guid Player1GameUserId { get; set; }
        public Guid Player1ClientId { get; set; }
        public string Player1Name { get; set; }
        public Guid Player2GameUserId { get; set; }
        public Guid Player2ClientId { get; set; }
        public string Player2Name { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string GameRoomStatus { get; set; }
        public Guid? WinnerGameUserId { get; set; }
    }
}
