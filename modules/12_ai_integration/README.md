# Module 12: AI/ML Integration with Semantic Kernel

## 🎯 Learning Objectives

After completing this module, you will be able to:
- Understand AI/ML integration patterns in .NET applications
- Use Semantic Kernel to integrate AI services
- Implement RAG (Retrieval Augmented Generation) patterns
- Build AI-powered APIs and services
- Use Azure OpenAI and other LLM providers
- Implement embeddings and vector search
- Apply AI responsibly with safety and ethics considerations

## 📚 Topics Covered

### 1. Introduction to AI Integration
- AI/ML landscape overview
- Large Language Models (LLMs)
- Use cases for AI in web services
- Choosing the right AI service
- Cost and performance considerations

### 2. Semantic Kernel Framework
- What is Semantic Kernel?
- Core concepts: Plugins, Functions, Memory
- Integrating with OpenAI, Azure OpenAI, and other providers
- Semantic functions vs native functions
- Planner and orchestration

### 3. Building AI-Powered APIs
- Designing AI endpoints
- Streaming responses
- Managing tokens and costs
- Caching strategies
- Error handling and fallbacks

### 4. RAG (Retrieval Augmented Generation)
- What is RAG and why use it?
- Vector embeddings and similarity search
- Vector databases (Azure AI Search, Qdrant, Chroma)
- Document chunking strategies
- Implementing semantic search

### 5. Memory and Context Management
- Semantic memory vs volatile memory
- Conversation history
- Context window management
- Memory stores and persistence

### 6. AI Safety and Ethics
- Content filtering and moderation
- Bias and fairness
- Privacy and data protection
- Responsible AI guidelines
- Monitoring AI behavior

## 🛠️ Prerequisites

- Completed Module 02 (ASP.NET Core Fundamentals)
- Completed Module 04 (Entity Framework Core)
- Completed Module 10 (.NET Aspire Deep Dive)
- Understanding of REST APIs
- Basic understanding of AI/ML concepts (helpful but not required)

## 📖 Key Concepts

### Semantic Kernel Components

1. **Kernel**: The orchestrator of AI services
2. **Plugins**: Collections of related functions
3. **Functions**: Atomic units of work (semantic or native)
4. **Memory**: Stores and retrieves contextual information
5. **Planner**: Automatically creates execution plans

### RAG Architecture

```
User Query → Embedding → Vector Search → Context Retrieval
    ↓                                            ↓
   LLM ← Augmented Prompt ← Relevant Documents
    ↓
Response
```

## 💻 Hands-on Exercises

### Exercise 1: Hello Semantic Kernel
Create your first AI-powered API:
- Set up Semantic Kernel with OpenAI
- Create a simple chat completion endpoint
- Add conversation history

### Exercise 2: Build a Plugin
Create a custom plugin:
- Implement native functions (C# methods)
- Create semantic functions (prompts)
- Combine both in a single plugin
- Use the plugin in your API

### Exercise 3: Implement RAG Pattern
Build a document Q&A system:
- Index documents into a vector database
- Implement semantic search
- Augment prompts with retrieved context
- Return answers with source citations

### Exercise 4: Streaming Responses
- Implement server-sent events (SSE) for streaming
- Stream tokens as they're generated
- Handle connection management
- Add proper error handling

### Exercise 5: AI Service with Aspire
- Integrate AI services into Aspire orchestration
- Configure OpenAI connection strings
- Implement proper configuration management
- Monitor AI service calls in Aspire Dashboard

## 📝 Sample Code Structure

```
ai-service-demo/
├── src/
│   ├── AiService.Api/
│   │   ├── Controllers/
│   │   │   ├── ChatController.cs
│   │   │   └── DocumentsController.cs
│   │   ├── Plugins/
│   │   │   └── CustomPlugin.cs
│   │   ├── Services/
│   │   │   ├── SemanticKernelService.cs
│   │   │   └── VectorSearchService.cs
│   │   └── Program.cs
│   ├── AiService.Core/
│   │   ├── Models/
│   │   └── Interfaces/
│   └── ServiceDefaults/
├── AppHost/
│   └── Program.cs (Aspire orchestration)
└── tests/
    └── AiService.Tests/
```

## 🔧 Sample Implementation

### Basic Semantic Kernel Setup

```csharp
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add Semantic Kernel
builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration["AzureOpenAI:DeploymentName"]!,
        endpoint: builder.Configuration["AzureOpenAI:Endpoint"]!,
        apiKey: builder.Configuration["AzureOpenAI:ApiKey"]!
    );

var app = builder.Build();

// Simple chat endpoint
app.MapPost("/api/chat", async (
    string message,
    Kernel kernel) =>
{
    var result = await kernel.InvokePromptAsync(
        $"You are a helpful assistant. User: {message}");
    
    return Results.Ok(new { response = result.ToString() });
});

app.Run();
```

### RAG Implementation Example

```csharp
public class DocumentQAService
{
    private readonly Kernel _kernel;
    private readonly IVectorStore _vectorStore;

    public async Task<string> AskQuestionAsync(string question)
    {
        // 1. Convert question to embedding
        var questionEmbedding = await _kernel.GetEmbeddingAsync(question);
        
        // 2. Search for relevant documents
        var relevantDocs = await _vectorStore.SearchAsync(
            questionEmbedding, 
            topK: 5);
        
        // 3. Build context from documents
        var context = string.Join("\n\n", 
            relevantDocs.Select(d => d.Content));
        
        // 4. Create augmented prompt
        var prompt = $@"
            Context:
            {context}
            
            Question: {question}
            
            Answer the question based on the context provided.
            If the answer is not in the context, say so.
        ";
        
        // 5. Get response from LLM
        var result = await _kernel.InvokePromptAsync(prompt);
        return result.ToString();
    }
}
```

## 📚 Additional Resources

- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/)
- [Azure OpenAI Service](https://learn.microsoft.com/azure/cognitive-services/openai/)
- [OpenAI Platform Documentation](https://platform.openai.com/docs)
- [RAG Pattern Guide](https://www.pinecone.io/learn/retrieval-augmented-generation/)
- [Responsible AI Resources](https://www.microsoft.com/ai/responsible-ai)
- [Vector Database Comparison](https://github.com/semantic-kernel/semantic-kernel/tree/main/docs)

## 🎓 Best Practices

1. **Token Management**: Monitor and optimize token usage to control costs
2. **Caching**: Cache responses for identical queries
3. **Streaming**: Use streaming for better user experience with long responses
4. **Error Handling**: Always have fallback mechanisms
5. **Content Filtering**: Implement both input and output filtering
6. **Rate Limiting**: Protect your API and manage costs
7. **Monitoring**: Track AI service usage, costs, and performance
8. **Testing**: Create comprehensive tests including edge cases
9. **Documentation**: Clearly document AI behavior and limitations
10. **Privacy**: Never send sensitive data to external AI services without proper controls

## ⚠️ Common Pitfalls

- **Not managing context window size**: LLMs have token limits
- **Ignoring costs**: AI services can be expensive at scale
- **Over-reliance on AI**: Not all problems need AI solutions
- **Poor prompt engineering**: Quality of prompts directly affects results
- **Not validating outputs**: Always validate AI-generated content
- **Ignoring latency**: AI services can be slow; design accordingly

## ✅ Module Checklist

- [ ] Understand AI/ML integration patterns
- [ ] Set up Semantic Kernel with OpenAI/Azure OpenAI
- [ ] Create custom plugins and functions
- [ ] Implement RAG pattern with vector search
- [ ] Build streaming AI responses
- [ ] Implement proper error handling and fallbacks
- [ ] Add content filtering and safety measures
- [ ] Monitor costs and performance
- [ ] Complete all exercises

## 🚀 Next Module

Continue to [Module 13: Performance Optimization](../13_performance/) to learn about optimizing your .NET applications.
