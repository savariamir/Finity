# Fault tolerance library designed for .Net core

Finity is a lightweight fault tolerance library designed to isolate access to remote resources and services. In a distributed environment, calls to remote resources and services can fail due to transient faults, such as slow network connections, timeouts, or the resources being overcommitted or temporarily unavailable.


```c#
    
    // Request
    public class QueryArticle
    {
        public int ArticleId { get; set; }
    }
    
    // Response
    public class ArticleReadModel
    {
        public string Title { get; set; }
        
        public string Content { get; set; }
    }
```
