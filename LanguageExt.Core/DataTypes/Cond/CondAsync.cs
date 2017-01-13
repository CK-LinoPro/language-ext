﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Conditional asynchronous computation.  Represents a computation that could 
    /// succeed or fail.  The result of the computation is an Option data-type promise.
    /// </summary>
    /// <typeparam name="A">Input type to the conditional computation</typeparam>
    /// <typeparam name="B">Resulting value type on success</typeparam>
    /// <param name="input">Input value type</param>
    /// <returns>Promise to return an optional result</returns>
    public delegate Task<Option<B>> CondAsync<A, B>(A input);

    public static partial class Prelude
    {
        /// <summary>
        /// Conditional asynchronous computation constructor.  Represents a computation 
        /// that could succeed or fail.  The result of the computation is an Option 
        /// data-type. Use the fluent API methods of Then and Else to extract the monadic 
        /// value.
        /// </summary>
        /// <typeparam name="A">Input type to the conditional computation</typeparam>
        /// <param name="pred">Predicate to apply to the input value</param>
        /// <returns>Conditional computation</returns>
        public static CondAsync<A, A> Cond<A>(Func<A, Task<bool>> pred) =>
            (A input) =>
                pred(input).ContinueWith(task =>
                    task.Result
                        ? Optional(input)
                        : Option<A>.None);
    }

    /// <summary>
    /// Extensions to the CondAsync type
    /// </summary>
    public static class CondAsyncExt
    {
        /// <summary>
        /// Apply a value as the first argument to the function provided.  
        /// </summary>
        /// <remarks>This is a general case apply function, however it is especially 
        /// useful for fluently applying a value to the result of the CondAsync.Else() 
        /// extension method.
        /// </remarks>
        /// <typeparam name="A">Type of the value to apply to the function</typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="a">Value to apply to the function</param>
        /// <param name="f">Function to apply the value to</param>
        /// <returns>Promise to return the result of applying the value to the function</returns>
        public static Task<B> Apply<A, B>(this A a, Func<A, Task<B>> cond) =>
            cond(a);

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="f">The 'then' computation</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Then<A, B>(this CondAsync<A, A> self, Func<A, B> f) =>
            self.Select(f);

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="f">The 'then' computation</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Then<A, B>(this CondAsync<A, A> self, Func<B> f) =>
            self.Select(_ => f());

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="value">The 'then' value</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Then<A, B>(this CondAsync<A, A> self, B value) =>
            self.Select(_ => value);

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="f">The 'then' computation</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Then<A, B>(this CondAsync<A, A> self, Func<A, Task<B>> f) =>
            self.Select(f);

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="f">The 'then' computation</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Then<A, B>(this CondAsync<A, A> self, Func<Task<B>> f) =>
            self.Select(_ => f());

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <remarks>
        /// The Then task will always run, even if not needed by the computation.  Only
        /// use this variant if the Then task is a Task.FromResult(x) or you absolutely
        /// definitely need the operation to run in parallel to the If/Else computation
        /// so that the result is available as quickly as possible.
        /// </remarks>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="value">The 'then' value</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Then<A, B>(this CondAsync<A, A> self, Task<B> value) =>
            self.Select(_ => value);

        /// <summary>
        /// A conditional computation where the input value can match any of the predicates
        /// provided to return a positive Some/True state.  This is like an if(a || b || c ...)
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="predicates">The predicates to test the bound value with</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Any<A, B>(this CondAsync<A, B> self, params Func<B, Task<bool>>[] predicates) =>
            input =>
                from x in self(input)
                from y in x.Match(
                    Some: b  => Tasks.Exists(predicates.Map(p => p(b)), identity),
                    None: () => Task.FromResult(false))
                select y
                    ? x
                    : None;

        /// <summary>
        /// A conditional computation where the input value must match all of the predicates
        /// provided to return a positive Some/True state.  This is like an if(a && b && c ...)
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="predicates">The predicates to test the bound value with</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> All<A, B>(this CondAsync<A, B> self, params Func<B, Task<bool>>[] predicates) =>
            input =>
                from x in self(input)
                from y in x.Match(
                    Some: b => Tasks.ForAll(predicates.Map(p => p(b)), identity),
                    None: () => Task.FromResult(false))
                select y
                    ? x
                    : None;

        /// <summary>
        /// A conditional computation where the input value can match any of the predicates
        /// provided to return a positive Some/True state.  This is like an if(a || b || c ...)
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="predicates">The predicates to test the bound value with</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Any<A, B>(this CondAsync<A, B> self, params Func<B, bool>[] predicates) =>
            env =>
                self(env).BindAsync(
                    input =>
                    {
                        foreach (var pred in predicates)
                        {
                            if (pred(input))
                            {
                                return Optional(input);
                            }
                        }
                        return None;
                    });

        /// <summary>
        /// A conditional computation where the input value must match all of the predicates
        /// provided to return a positive Some/True state.  This is like an if(a && b && c ...)
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="predicates">The predicates to test the bound value with</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> All<A, B>(this CondAsync<A, B> self, params Func<B, bool>[] predicates) =>
            env =>
                self(env).BindAsync(
                    input =>
                    {
                        foreach (var pred in predicates)
                        {
                            if (!pred(input))
                            {
                                return None;
                            }
                        }
                        return Optional(input);
                    });

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="Else">The Else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        public static Func<A, Task<B>> Else<A, B>(this CondAsync<A, B> self, Func<A, B> Else) =>
            input =>
                self(input).ContinueWith(b => b.Result.IfNone(() => Else(input)));

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="Else">The Else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        public static Func<A, Task<B>> Else<A, B>(this CondAsync<A, B> self, Func<B> Else) =>
            input =>
                self(input).ContinueWith(b => b.Result.IfNone(() => Else()));

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="Else">The Else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        public static Func<A, Task<B>> Else<A, B>(this CondAsync<A, B> self, B Else) =>
            input =>
                self(input).ContinueWith(b => b.Result.IfNone(Else));

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="Else">The Else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        public static Func<A, Task<B>> Else<A, B>(this CondAsync<A, B> self, Func<A, Task<B>> Else) =>
            input =>
                from b in self(input)
                from c in b.Match(
                    Some: c1 => c1.AsTask(),
                    None: () => Else(input))
                select c;

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="Else">The Else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        public static Func<A, Task<B>> Else<A, B>(this CondAsync<A, B> self, Func<Task<B>> Else) =>
            input =>
                from b in self(input)
                from c in b.Match(
                    Some: c1 => c1.AsTask(),
                    None: () => Else())
                select c;

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <remarks>
        /// The Else task will always run, even if not needed by the computation.  Only
        /// use this variant if the Else task is a Task.FromResult(x) or you absolutely
        /// definitely need the operation to run in parallel to the If/Then computation
        /// so that the result is available as quickly as possible.
        /// </remarks>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="Else">The Else value to use if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        public static Func<A, Task<B>> Else<A, B>(this CondAsync<A, B> self, Task<B> Else) =>
            input =>
                from b in self(input)
                from c in b.Match(
                    Some: c1 => c1.AsTask(),
                    None: () => Else)
                select c;

        /// <summary>
        /// Functor map of the conditional computation
        /// </summary>
        /// <typeparam name="A">Source computation input type</typeparam>
        /// <typeparam name="B">Source computation output type</typeparam>
        /// <typeparam name="C">Mapped computation output type</typeparam>
        /// <param name="self">The conditional computation to map</param>
        /// <param name="map">Functor mapping function</param>
        /// <returns>A mapped conditional computation</returns>
        public static CondAsync<A, C> Map<A, B, C>(this CondAsync<A, B> self, Func<B, Task<C>> map) =>
            Select(self, map);

        /// <summary>
        /// Functor map of the conditional computation
        /// </summary>
        /// <typeparam name="A">Source computation input type</typeparam>
        /// <typeparam name="B">Source computation output type</typeparam>
        /// <typeparam name="C">Mapped computation output type</typeparam>
        /// <param name="self">The conditional computation to map</param>
        /// <param name="map">Functor mapping function</param>
        /// <returns>A mapped conditional computation</returns>
        public static CondAsync<A, C> Map<A, B, C>(this CondAsync<A, B> self, Func<B, C> map) =>
            Select(self, map);

        /// <summary>
        /// Functor map of the conditional computation
        /// </summary>
        /// <typeparam name="A">Source computation input type</typeparam>
        /// <typeparam name="B">Source computation output type</typeparam>
        /// <typeparam name="C">Mapped computation output type</typeparam>
        /// <param name="self">The conditional computation to map</param>
        /// <param name="map">Functor mapping function</param>
        /// <returns>A mapped conditional computation</returns>
        public static CondAsync<A, C> Select<A, B, C>(this CondAsync<A, B> self, Func<B, Task<C>> map) =>
            input =>
                from b in self(input)
                from c in b.Match(
                    Some: b2 => map(b2).ContinueWith(b3 => Optional(b3.Result)),
                    None: () => Task.FromResult(Option<C>.None))
                select c;

        /// <summary>
        /// Functor map of the conditional computation
        /// </summary>
        /// <typeparam name="A">Source computation input type</typeparam>
        /// <typeparam name="B">Source computation output type</typeparam>
        /// <typeparam name="C">Mapped computation output type</typeparam>
        /// <param name="self">The conditional computation to map</param>
        /// <param name="map">Functor mapping function</param>
        /// <returns>A mapped conditional computation</returns>
        public static CondAsync<A, C> Select<A, B, C>(this CondAsync<A, B> self, Func<B, C> map) =>
            input =>
                from b in self(input)
                select b.Map(map);

        /// <summary>
        /// Monadic bind for conditional computations.  Allows nesting of computations 
        /// that follow the rules of Cond, namely that a None/False result cancels the
        /// operation until an Else is encountered.
        /// </summary>
        public static CondAsync<A, D> SelectMany<A, B, C, D>(
            this CondAsync<A, B> self,
            Func<B, CondAsync<A, C>> bind,
            Func<B, C, D> project) =>
                input =>
                    from b in self(input)
                    from c in b.Match(
                        Some: b1 => bind(b1)(input),
                        None: () => Option<C>.None.AsTask())
                    select (from x in b
                            from y in c
                            select project(x,y));

        /// <summary>
        /// Filter the conditional computation.  This is the equivalent of the predicate
        /// in an If statement.
        /// </summary>
        /// <typeparam name="A">The input type of the conditional computation</typeparam>
        /// <typeparam name="B">The output type of the conditional computation</typeparam>
        /// <param name="self">The conditional computation to test</param>
        /// <param name="pred">The predicate function</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Filter<A, B>(this CondAsync<A, B> self, Func<B, bool> pred) =>
            input =>
                from b in self(input)
                select b.Map(pred).IfNone(false) ? b : None;

        /// <summary>
        /// Filter the conditional computation.  This is the equivalent of the predicate
        /// in an If statement.
        /// </summary>
        /// <typeparam name="A">The input type of the conditional computation</typeparam>
        /// <typeparam name="B">The output type of the conditional computation</typeparam>
        /// <param name="self">The conditional computation to test</param>
        /// <param name="pred">The predicate function</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Filter<A, B>(this CondAsync<A, B> self, Func<B, Task<bool>> pred) =>
            input =>
                from b in self(input)
                from p in b.Map(pred).IfNone(false.AsTask())
                select p ? b : None;

        /// <summary>
        /// Filter the conditional computation.  This is the equivalent of the predicate
        /// in an If statement.
        /// </summary>
        /// <typeparam name="A">The input type of the conditional computation</typeparam>
        /// <typeparam name="B">The output type of the conditional computation</typeparam>
        /// <param name="self">The conditional computation to test</param>
        /// <param name="pred">The predicate function</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Where<A, B>(this CondAsync<A, B> self, Func<B, bool> pred) =>
            input =>
                from b in self(input)
                select b.Map(pred).IfNone(false) ? b : None;

        /// <summary>
        /// Filter the conditional computation.  This is the equivalent of the predicate
        /// in an If statement.
        /// </summary>
        /// <typeparam name="A">The input type of the conditional computation</typeparam>
        /// <typeparam name="B">The output type of the conditional computation</typeparam>
        /// <param name="self">The conditional computation to test</param>
        /// <param name="pred">The predicate function</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Where<A, B>(this CondAsync<A, B> self, Func<B, Task<bool>> pred) =>
            input =>
                from b in self(input)
                from p in b.Map(pred).IfNone(false.AsTask())
                select p ? b : None;
    }

    public static class CondSyncToAsyncExt
    {
        /// <summary>
        /// Converts a synchronous conditional computation to an asynchronous one
        /// </summary>
        /// <typeparam name="A">Type of the computation input value</typeparam>
        /// <typeparam name="B">Type of the computation output value</typeparam>
        /// <param name="self">The synchronous computation to convert</param>
        /// <returns></returns>
        public static CondAsync<A, B> ToAsync<A, B>(this Cond<A, B> self) =>
            input =>
                Task.Run(() => self(input));

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="f">The 'then' computation</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Then<A, B>(this Cond<A, A> self, Func<A, Task<B>> f) =>
            self.ToAsync().Select(f);

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="f">The 'then' computation</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Then<A, B>(this Cond<A, A> self, Func<Task<B>> f) =>
            self.Select(_ => f());

        /// <summary>
        /// Provide the behaviour to run if the condition of the Cond computation
        /// is in a Some/True state.  This is equivalent to the 'then' part of an If/Then/Else
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to compose with</param>
        /// <param name="value">The 'then' value</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Then<A, B>(this Cond<A, A> self, Task<B> value) =>
            self.Select(_ => value);

        /// <summary>
        /// A conditional computation where the input value can match any of the predicates
        /// provided to return a positive Some/True state.  This is like an if(a || b || c ...)
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="predicates">The predicates to test the bound value with</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Any<A, B>(this Cond<A, B> self, params Func<B, Task<bool>>[] predicates) =>
            input =>
                from x in self.ToAsync()(input)
                from y in x.Match(
                    Some: b => Tasks.Exists(predicates.Map(p => p(b)), identity),
                    None: () => Task.FromResult(false))
                select y
                    ? x
                    : None;

        /// <summary>
        /// A conditional computation where the input value must match all of the predicates
        /// provided to return a positive Some/True state.  This is like an if(a && b && c ...)
        /// operation.
        /// </summary>
        /// <typeparam name="A">Type of the value that is input to the computation</typeparam>
        /// <typeparam name="B">Type of the value that results from this computation</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="predicates">The predicates to test the bound value with</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> All<A, B>(this Cond<A, B> self, params Func<B, Task<bool>>[] predicates) =>
            input =>
                from x in self.ToAsync()(input)
                from y in x.Match(
                    Some: b => Tasks.ForAll(predicates.Map(p => p(b)), identity),
                    None: () => Task.FromResult(false))
                select y
                    ? x
                    : None;

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="Else">The Else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        public static Func<A, Task<B>> Else<A, B>(this Cond<A, B> self, Func<A, Task<B>> Else) =>
            input =>
                from b in self.ToAsync()(input)
                from c in b.Match(
                    Some: c1 => c1.AsTask(),
                    None: () => Else(input))
                select c;

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="Else">The Else function to run if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        public static Func<A, Task<B>> Else<A, B>(this Cond<A, B> self, Func<Task<B>> Else) =>
            input =>
                from b in self.ToAsync()(input)
                from c in b.Match(
                    Some: c1 => c1.AsTask(),
                    None: () => Else())
                select c;

        /// <summary>
        /// Builds a delegate that runs the conditional computation, taking an input
        /// value and returning a result value.  This would usually be the last thing
        /// in a fluent Cond computation.
        /// </summary>
        /// <remarks>
        /// The Else task will always run, even if not needed by the computation.  Only
        /// use this variant if the Else task is a Task.FromResult(x) or you absolutely
        /// definitely need the operation to run in parallel to the If/Then computation
        /// so that the result is available as quickly as possible.
        /// </remarks>
        /// <typeparam name="A">Type of the input value</typeparam>
        /// <typeparam name="B">Type of the result value</typeparam>
        /// <param name="self">The Cond computation to test</param>
        /// <param name="Else">The Else task to use if the Cond computation ends up
        /// in a None/False state.</param>
        /// <returns>The result of the conditional computation</returns>
        public static Func<A, Task<B>> Else<A, B>(this Cond<A, B> self, Task<B> Else) =>
            input =>
                from b in self.ToAsync()(input)
                from c in b.Match(
                    Some: c1 => c1.AsTask(),
                    None: () => Else)
                select c;

        /// <summary>
        /// Functor map of the conditional computation
        /// </summary>
        /// <typeparam name="A">Source computation input type</typeparam>
        /// <typeparam name="B">Source computation output type</typeparam>
        /// <typeparam name="C">Mapped computation output type</typeparam>
        /// <param name="self">The conditional computation to map</param>
        /// <param name="map">Functor mapping function</param>
        /// <returns>A mapped conditional computation</returns>
        public static CondAsync<A, C> Map<A, B, C>(this Cond<A, B> self, Func<B, Task<C>> map) =>
            Select(self, map);

        /// <summary>
        /// Functor map of the conditional computation
        /// </summary>
        /// <typeparam name="A">Source computation input type</typeparam>
        /// <typeparam name="B">Source computation output type</typeparam>
        /// <typeparam name="C">Mapped computation output type</typeparam>
        /// <param name="self">The conditional computation to map</param>
        /// <param name="map">Functor mapping function</param>
        /// <returns>A mapped conditional computation</returns>
        public static CondAsync<A, C> Select<A, B, C>(this Cond<A, B> self, Func<B, Task<C>> map) =>
            input =>
                from b in self.ToAsync()(input)
                from c in b.Match(
                    Some: b2 => map(b2).ContinueWith(b3 => Optional(b3.Result)),
                    None: () => Task.FromResult(Option<C>.None))
                select c;

        /// <summary>
        /// Monadic bind for conditional computations.  Allows nesting of computations 
        /// that follow the rules of Cond, namely that a None/False result cancels the
        /// operation until an Else is encountered.
        /// </summary>
        public static CondAsync<A, D> SelectMany<A, B, C, D>(
            this Cond<A, B> self,
            Func<B, CondAsync<A, C>> bind,
            Func<B, C, D> project) =>
                input =>
                    from b in self.ToAsync()(input)
                    from c in b.Match(
                        Some: b1 => bind(b1)(input),
                        None: () => Option<C>.None.AsTask())
                    select (from x in b
                            from y in c
                            select project(x, y));

        /// <summary>
        /// Filter the conditional computation.  This is the equivalent of the predicate
        /// in an If statement.
        /// </summary>
        /// <typeparam name="A">The input type of the conditional computation</typeparam>
        /// <typeparam name="B">The output type of the conditional computation</typeparam>
        /// <param name="self">The conditional computation to test</param>
        /// <param name="pred">The predicate function</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Filter<A, B>(this Cond<A, B> self, Func<B, Task<bool>> pred) =>
            input =>
                from b in self.ToAsync()(input)
                from p in b.Map(pred).IfNone(false.AsTask())
                select p ? b : None;

        /// <summary>
        /// Filter the conditional computation.  This is the equivalent of the predicate
        /// in an If statement.
        /// </summary>
        /// <typeparam name="A">The input type of the conditional computation</typeparam>
        /// <typeparam name="B">The output type of the conditional computation</typeparam>
        /// <param name="self">The conditional computation to test</param>
        /// <param name="pred">The predicate function</param>
        /// <returns>A conditional computation</returns>
        public static CondAsync<A, B> Where<A, B>(this Cond<A, B> self, Func<B, Task<bool>> pred) =>
            input =>
                from b in self.ToAsync()(input)
                from p in b.Map(pred).IfNone(false.AsTask())
                select p ? b : None;
    }

    static class Tasks
    {
        public static async Task<bool> ForAll<A, B>(A a, IEnumerable<Func<A, B>> fs, Func<B, bool> pred)
        {
            var tasks = fs.Map(f => Task.Run(() => f(a))).ToList();

            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                if (!pred(completed.Result))
                {
                    return false;
                }
                tasks.Remove(completed);
            }
            return true;
        }

        public static async Task<bool> ForAll<A>(IEnumerable<Task<A>> fs, Func<A, bool> pred)
        {
            var tasks = fs.ToList();

            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                if (!pred(completed.Result))
                {
                    return false;
                }
                tasks.Remove(completed);
            }
            return true;
        }

        public static async Task<bool> Exists<A, B>(A a, IEnumerable<Func<A, B>> fs, Func<B, bool> pred)
        {
            var tasks = fs.Map(f => Task.Run(() => f(a))).ToList();

            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                var res = pred(completed.Result);
                if (res)
                {
                    return res;
                }
                tasks.Remove(completed);
            }

            return false;
        }

        public static async Task<bool> Exists<A>(IEnumerable<Task<A>> fs, Func<A, bool> pred)
        {
            var tasks = fs.ToList();

            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                var res = pred(completed.Result);
                if (res)
                {
                    return res;
                }
                tasks.Remove(completed);
            }
            return false;
        }
    }
}