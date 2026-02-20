namespace Bot.Entities;

public delegate Task MessageCallback(IUserMessage message);
public delegate Task AsyncFunction();