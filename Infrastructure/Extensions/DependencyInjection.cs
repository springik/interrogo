using Core.Entities;
using Core.Interfaces;
using Infrastructure.MongoDb;
using Infrastructure.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddScoped<IRealTimeVotingService, RealTimeVotingService>();
        
        ConfigureMongoDbMapping();
        
        var mongoConnectionString = configuration.GetConnectionString("MongoDb") ?? "localhost:27017";
        var mongoClient = new MongoClient(mongoConnectionString);
        var mongoDatabase = mongoClient.GetDatabase("interrogo");
        
        services.AddSingleton(mongoDatabase);
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        
        return services;
    }

    private static void ConfigureMongoDbMapping()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Event)))
        {
            BsonClassMap.RegisterClassMap<Event>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c =>c.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                }
            );
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Question)))
        {
            BsonClassMap.RegisterClassMap<Question>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                }
            );
        }
    }
}