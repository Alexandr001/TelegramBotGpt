﻿namespace Main.View.Factories;

public interface IFactory<TObj, TClass>
		where TClass : class
{
	IReadOnlyDictionary<TObj, TClass> RouteAndObject { get; }
	TClass? FactoryMethod(string str);
}