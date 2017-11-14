using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

[TestFixture]
public class BirdTests
{
    const float DeltaTime = 1f / 60f;
    const float InitialPosition = 0f;
    const float Gravity = -10f;
    const float JumpBoost = 50f;

    Bird bird;
    State[] states;

    [Test]
    public void WhenTImePassesBirdFalls()
    {
        GivenABird();
        WhenTimePasses(frames: 2, deltaTime: DeltaTime);

        ThenBirdFalls();
    }

    [Test]
    public void WhenTimePassesVelocityIsApplied()
    {
        GivenABird();
        WhenTimePasses(frames: 5, deltaTime: DeltaTime);

        ThenBirdVerticalPositionChangesWithVelocity();
    }

    [Test]
    public void WhenTimePassesFallAccelerationIsApplied()
    {
        GivenABird();
        WhenTimePasses(frames: 5, deltaTime: DeltaTime);

        ThenBirdFallAccelerates();
    }

    [Test]
    public void WhenUserTapsAndTimePassesBirdJumps()
    {
        GivenABird();
        WhenBirdJumpsAndTimePasses(frames: 5, deltaTime: DeltaTime);

        ThenBirdJumps();
    }

    [Test]
    public void WhenBirdCollidesWithSomethingItCantJump()
    {
        GivenABird();

        WhenBirdRecievesACollisionAndJumps();

        ThenBirdDoesntJump();
    }

    [Test]
    public void WhenBirdCollidesWithSomethingItImmediatelyStartsFalling()
    {
        GivenABird();

        WhenBirdRecievesACollision();

        ThenBirdImmediatelyStartsFalling();
    }

    [Test]
    public void WhenBirdCollidesWithSomethingItFalls()
    {
        GivenABird();

        WhenBirdRecievesACollisionAndTimePasses();

        ThenBirdFallAccelerates();
    }

    [Test]
    public void WhenBirdCollidesForASecondTimeThenDontStartFallingAgain()
    {
        GivenABirdThatCollidedAWhileAgo();

        WhenBirdRecievesACollision();

        ThenBirdVelocityIsNotZero();
    }

    void GivenABird()
    {
        bird = new Bird(InitialPosition, Gravity, JumpBoost);
    }
    
    void GivenABirdThatCollidedAWhileAgo()
    {
        GivenABird();
        WhenBirdRecievesACollisionAndTimePasses();
    }

    void WhenTimePasses(int frames, float deltaTime)
    {
        var statesInTime = new State[frames];

        for (var i = 0; i < statesInTime.Length; i++)
        {
            statesInTime[i] = bird.Update(deltaTime);
        }

        states = statesInTime;
    }

    void WhenBirdJumpsAndTimePasses(int frames, float deltaTime)
    {
        WhenBirdJumps();
        WhenTimePasses(frames, deltaTime);
    }

    void WhenBirdRecievesACollisionAndJumps()
    {
        WhenBirdRecievesACollision();
        WhenBirdJumps();
    }

    void WhenBirdRecievesACollision()
    {
        bird.Collide();
    }

    void WhenBirdRecievesACollisionAndTimePasses()
    {
        WhenBirdRecievesACollision();
        WhenTimePasses(frames: 5, deltaTime: DeltaTime);
    }

    void ThenBirdImmediatelyStartsFalling()
    {
        Assert.AreEqual(bird.VerticalVelocity, 0f);
    }


    void WhenBirdJumps()
    {
        bird.Jump();
    }

    void ThenBirdFalls()
    {
        Assert.IsTrue(states[0].position > states[1].position);
    }

    void ThenBirdVerticalPositionChangesWithVelocity()
    {
        for (var i = 1; i < states.Length; i++)
        {
            var deltaPosition = states[i].position - states[i - 1].position;
            var expectedDeltaPosition = states[i].velocity * DeltaTime;
            
            Assert.IsTrue(Mathf.Approximately(expectedDeltaPosition, deltaPosition));
        }
    }

    void ThenBirdFallAccelerates()
    {
        for (var i = 1; i < states.Length; i++)
        {
            var deltaVelocity = states[i].velocity - states[i - 1].velocity;
            var expected = Gravity * DeltaTime;

            Assert.IsTrue(Mathf.Approximately(expected, deltaVelocity));
        }
    }

    void ThenBirdJumps()
    {
        for (int i = 0; i < states.Length; i++)
            Assert.IsTrue(Mathf.Approximately(states[i].velocity, JumpBoost + Gravity * (i+1) * DeltaTime));            
    }

    void ThenBirdDoesntJump()
    {
        Assert.AreNotEqual(bird.VerticalVelocity, JumpBoost);
    }

    void ThenBirdVelocityIsNotZero()
    {
        Assert.AreNotEqual(bird.VerticalVelocity, 0);
    }
}
