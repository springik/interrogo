using Core.Entities;
using Core.Interfaces;
using Infrastructure.MongoDb;
using Infrastructure.Neo4j;
using Infrastructure.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Neo4j.Driver;
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
        
        var neo4jUri = configuration.GetConnectionString("Neo4j") ?? "bolt://localhost:7687";
        var neo4jDriver = GraphDatabase.Driver(neo4jUri, AuthTokens.Basic("neo4j", "qQsFSo3q30EdqvZOM6zX"));
        services.AddSingleton(neo4jDriver);
        services.AddScoped<IGraphRecommendationService, GraphRecommendationService>();
        
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