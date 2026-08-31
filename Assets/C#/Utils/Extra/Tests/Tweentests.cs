// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
// using ITween;
//
// // Requires the Unity Test Framework package and must live in an assembly
// // that references UnityEngine.TestRunner / UnityEditor.TestRunner (PlayMode tests folder).
// public class TweenTests
// {
//     private GameObject _mover;
//     private GameObject _target;
//
//     [SetUp]
//     public void SetUp()
//     {
//         _mover = new GameObject("Mover");
//         _target = new GameObject("Target");
//     }
//
//     [TearDown]
//     public void TearDown()
//     {
//         Object.Destroy(_mover);
//         Object.Destroy(_target);
//         Time.timeScale = 1f;
//     }
//
//     // Advances frames until the tween dies or the frame budget runs out.
//     // The frame cap exists so a broken tween (never completes) fails the test instead of hanging it.
//     private static IEnumerator WaitForCompletion(Tween tween, int maxFrames = 300)
//     {
//         int frames = 0;
//         while (tween.IsAlive && frames < maxFrames)
//         {
//             frames++;
//             yield return null;
//         }
//     }
//
//     // ===== Basic settings =====
//
//     [UnityTest]
//     public IEnumerator LinearMove_ReachesTargetPosition()
//     {
//         _target.transform.position = new Vector3(5f, 0f, 0f);
//
//         var settings = new TweenSettings(0.2f, EasingType.Linear);
//
//         bool completed = false;
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings, () => completed = true);
//         tween.Start();
//
//         yield return WaitForCompletion(tween);
//
//         Assert.IsTrue(completed);
//         Assert.AreEqual(_target.transform.position, _mover.transform.position);
//     }
//
//     [UnityTest]
//     public IEnumerator DelayTime_DelaysMovementStart()
//     {
//         _target.transform.position = new Vector3(3f, 0f, 0f);
//         Vector3 startPos = _mover.transform.position;
//
//         var settings = new TweenSettings(0.2f, EasingType.Linear, delayTime: 0.3f, loopingType: TweenSettings.LoopType.Single, loopCount: 1);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         tween.Start();
//
//         yield return new WaitForSeconds(0.15f);
//         Assert.AreEqual(startPos, _mover.transform.position); // still inside the delay window
//
//         yield return WaitForCompletion(tween);
//         Assert.AreEqual(_target.transform.position, _mover.transform.position);
//     }
//
//     [UnityTest]
//     public IEnumerator Overshoot_OutBack_ExceedsTargetValueMidway()
//     {
//         var settings = new TweenSettings(0.1f, EasingType.OutBack, 1.70158f);
//
//         float maxValue = 0f;
//         Tween tween = TweenManager.Value(_mover, 0f, 1f, settings, v => maxValue = Mathf.Max(maxValue, v));
//         tween.Start();
//
//         yield return WaitForCompletion(tween);
//
//         Assert.Greater(maxValue, 1f); // OutBack overshoots past the target before settling
//     }
//
//     [UnityTest]
//     public IEnumerator CustomCurve_UsesProvidedAnimationCurve()
//     {
//         AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
//         var settings = new TweenSettings(0.1f, EasingType.Custom, customCurve: curve);
//
//         float lastValue = 0f;
//         Tween tween = TweenManager.Value(_mover, 0f, 1f, settings, v => lastValue = v);
//         tween.Start();
//
//         yield return WaitForCompletion(tween);
//
//         Assert.AreEqual(1f, lastValue, 0.01f);
//     }
//
//     // ===== Looping =====
//
//     [UnityTest]
//     public IEnumerator Repeat_FiresLoopCompleteCorrectNumberOfTimes()
//     {
//         _target.transform.position = new Vector3(2f, 0f, 0f);
//
//         var settings = new TweenSettings(0.1f, EasingType.Linear, loopingType: TweenSettings.LoopType.Repeat, loopCount: 3);
//
//         int loopCompleteCount = 0;
//         bool completed = false;
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings, () => completed = true);
//         tween.OnLoopComplete += () => loopCompleteCount++;
//         tween.Start();
//
//         yield return WaitForCompletion(tween);
//
//         Assert.IsTrue(completed);
//         Assert.AreEqual(2, loopCompleteCount); // 3rd pass finishes via OnComplete, not OnLoopComplete
//     }
//
//     [UnityTest]
//     public IEnumerator PingPong_OneRoundTrip_ReturnsToStart()
//     {
//         Vector3 startPos = _mover.transform.position;
//         _target.transform.position = new Vector3(4f, 0f, 0f);
//
//         var settings = new TweenSettings(0.1f, EasingType.Linear, loopingType: TweenSettings.LoopType.PingPong, loopCount: 1);
//
//         int loopCompleteCount = 0;
//         bool completed = false;
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings, () => completed = true);
//         tween.OnLoopComplete += () => loopCompleteCount++;
//         tween.Start();
//
//         yield return WaitForCompletion(tween);
//
//         Assert.IsTrue(completed);
//         Assert.AreEqual(1, loopCompleteCount);
//         Assert.AreEqual(startPos, _mover.transform.position);
//     }
//
//     [UnityTest]
//     public IEnumerator HangTime_HoldsPositionBetweenLoops()
//     {
//         _target.transform.position = new Vector3(1f, 0f, 0f);
//
//         var settings = new TweenSettings(0.1f, EasingType.Linear,
//             loopingType: TweenSettings.LoopType.Repeat, loopCount: 2, hangTime: 0.3f);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         tween.Start();
//
//         yield return new WaitForSeconds(0.12f); // past the first loop's duration
//         Vector3 posAfterFirstLoop = _mover.transform.position;
//
//         yield return new WaitForSeconds(0.1f); // still inside hang time
//         Assert.AreEqual(posAfterFirstLoop, _mover.transform.position);
//
//         yield return WaitForCompletion(tween);
//     }
//
//     // ===== TweenOptions flags =====
//
//     [UnityTest]
//     public IEnumerator IgnoreTimeScale_True_StillProgressesWhenTimeScaleZero()
//     {
//         _target.transform.position = new Vector3(2f, 0f, 0f);
//
//         var flags = new TweenSettings.TweenOptions(ignoreTimeScale: true);
//         var settings = new TweenSettings(0.1f, EasingType.Linear, flags: flags);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         tween.Start();
//
//         Time.timeScale = 0f;
//         yield return WaitForCompletion(tween, maxFrames: 60);
//
//         Assert.IsFalse(tween.IsAlive);
//         Assert.AreEqual(_target.transform.position, _mover.transform.position);
//     }
//
//     [UnityTest]
//     public IEnumerator IgnoreTimeScale_False_FreezesWhenTimeScaleZero()
//     {
//         _target.transform.position = new Vector3(2f, 0f, 0f);
//
//         var settings = new TweenSettings(0.1f, EasingType.Linear);
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         tween.Start();
//
//         Time.timeScale = 0f;
//         for (int i = 0; i < 30; i++) yield return null;
//
//         Assert.IsTrue(tween.IsAlive); // no scaled time passed, should not have finished
//     }
//
//     [UnityTest]
//     public IEnumerator InvertStartingDirection_PlaysFromTargetBackToStart()
//     {
//         Vector3 startPos = _mover.transform.position;
//         _target.transform.position = new Vector3(3f, 0f, 0f);
//
//         var flags = new TweenSettings.TweenOptions(invertStartingDirection: true);
//         var settings = new TweenSettings(0.2f, EasingType.Linear, flags: flags);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         tween.Start();
//
//         yield return null;
//         float distToTarget = Vector3.Distance(_mover.transform.position, _target.transform.position);
//         float distToStart = Vector3.Distance(_mover.transform.position, startPos);
//         Assert.Less(distToTarget, distToStart); // begins near the target, not the start
//
//         yield return WaitForCompletion(tween);
//         Assert.AreEqual(startPos, _mover.transform.position); // ends back where it began
//     }
//
//     [UnityTest]
//     public IEnumerator IgnoreInvertEasing_ChangesEasingOnReverseLeg()
//     {
//         var flagsIgnore = new TweenSettings.TweenOptions(ignoreInvertEasing: true);
//         var settingsIgnore = new TweenSettings(0.1f, EasingType.InQuad, flags: flagsIgnore,
//             loopingType: TweenSettings.LoopType.PingPong, loopCount: 1);
//
//         var settingsNormal = new TweenSettings(0.1f, EasingType.InQuad,
//             loopingType: TweenSettings.LoopType.PingPong, loopCount: 1);
//
//         var valuesIgnore = new List<float>();
//         var valuesNormal = new List<float>();
//
//         Tween tweenIgnore = TweenManager.Value(_mover, 0f, 1f, settingsIgnore, v => valuesIgnore.Add(v));
//         tweenIgnore.Start();
//         yield return WaitForCompletion(tweenIgnore);
//
//         Tween tweenNormal = TweenManager.Value(_mover, 0f, 1f, settingsNormal, v => valuesNormal.Add(v));
//         tweenNormal.Start();
//         yield return WaitForCompletion(tweenNormal);
//
//         // Same curve, same duration, only the flag differs — the reverse leg should diverge.
//         CollectionAssert.AreNotEqual(valuesIgnore, valuesNormal);
//     }
//
//     [UnityTest]
//     public IEnumerator StartAutomatically_BeginsWithoutExplicitStart()
//     {
//         _target.transform.position = new Vector3(1f, 0f, 0f);
//
//         var flags = new TweenSettings.TweenOptions(startAutomatically: true);
//         var settings = new TweenSettings(0.1f, EasingType.Linear, flags: flags);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         // no tween.Start() call
//
//         Assert.IsTrue(tween.IsRunning);
//
//         yield return WaitForCompletion(tween);
//         Assert.AreEqual(_target.transform.position, _mover.transform.position);
//     }
//
//     [UnityTest]
//     public IEnumerator KillWhenStopped_StopKillsInsteadOfPausing()
//     {
//         var flags = new TweenSettings.TweenOptions(startAutomatically: true, killWhenStopped: true);
//         var settings = new TweenSettings(0.5f, EasingType.Linear, flags: flags);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         yield return null;
//
//         tween.Stop();
//
//         Assert.IsFalse(tween.IsAlive);
//     }
//
//     [UnityTest]
//     public IEnumerator CompleteWhenKilled_True_FiresOnCompleteOnManualKill()
//     {
//         var flags = new TweenSettings.TweenOptions(completeWhenKilled: true);
//         var settings = new TweenSettings(0.5f, EasingType.Linear, flags: flags);
//
//         bool completed = false;
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings, () => completed = true);
//         tween.Start();
//         yield return null;
//
//         tween.Kill();
//
//         Assert.IsTrue(completed);
//     }
//
//     [UnityTest]
//     public IEnumerator CompleteWhenKilled_False_DoesNotFireOnCompleteOnManualKill()
//     {
//         var settings = new TweenSettings(0.5f, EasingType.Linear);
//
//         bool completed = false;
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings, () => completed = true);
//         tween.Start();
//         yield return null;
//
//         tween.Kill();
//
//         Assert.IsFalse(completed);
//     }
//
//     [UnityTest]
//     public IEnumerator ResetWhenStopped_StopResetsInsteadOfKilling()
//     {
//         var flags = new TweenSettings.TweenOptions(startAutomatically: true, resetWhenStopped: true);
//         var settings = new TweenSettings(0.2f, EasingType.Linear, flags: flags);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         yield return null;
//
//         tween.Stop();
//
//         Assert.IsTrue(tween.IsAlive);
//         Assert.IsFalse(tween.IsPaused);
//     }
//
//     // ===== Pause =====
//
//     [UnityTest]
//     public IEnumerator Pause_WithDuration_ResumesAfterTime()
//     {
//         _target.transform.position = new Vector3(2f, 0f, 0f);
//         var settings = new TweenSettings(0.3f, EasingType.Linear);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         tween.Start();
//         yield return null;
//
//         bool unpaused = false;
//         tween.Pause(0.15f, () => unpaused = true);
//         Assert.IsTrue(tween.IsPaused);
//
//         Vector3 posDuringPause = _mover.transform.position;
//         yield return new WaitForSeconds(0.05f);
//         Assert.AreEqual(posDuringPause, _mover.transform.position);
//
//         yield return new WaitForSeconds(0.2f); // past the pause duration
//         Assert.IsTrue(unpaused);
//         Assert.IsFalse(tween.IsPaused);
//
//         yield return WaitForCompletion(tween);
//     }
//
//     [UnityTest]
//     public IEnumerator Pause_WithPredicate_ResumesWhenConditionMet()
//     {
//         _target.transform.position = new Vector3(2f, 0f, 0f);
//         var settings = new TweenSettings(0.2f, EasingType.Linear);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         tween.Start();
//         yield return null;
//
//         bool gateOpen = false;
//         bool unpaused = false;
//         tween.Pause(() => gateOpen, () => unpaused = true);
//
//         yield return new WaitForSeconds(0.1f);
//         Assert.IsFalse(unpaused); // gate still closed
//
//         gateOpen = true;
//         yield return null; // predicate checked on next Update
//
//         Assert.IsTrue(unpaused);
//         yield return WaitForCompletion(tween);
//     }
//
//     // ===== Chaining regression (mid-enumeration Start/Stop from a callback) =====
//
//     [UnityTest]
//     public IEnumerator ChainedTweens_OnCompleteStartingAnotherTween_DoesNotThrow()
//     {
//         GameObject targetB = new GameObject("TargetB");
//         targetB.transform.position = new Vector3(1f, 0f, 0f);
//
//         var settingsA = new TweenSettings(0.05f, EasingType.Linear);
//         var settingsB = new TweenSettings(0.05f, EasingType.Linear);
//
//         bool bCompleted = false;
//         Tween tweenB = _target.transform.IT_Move(targetB.transform, settingsB, () => bCompleted = true);
//
//         Tween tweenA = _mover.transform.IT_Move(_target.transform, settingsA, () => tweenB.Start());
//         tweenA.Start();
//
//         yield return WaitForCompletion(tweenA);
//         yield return WaitForCompletion(tweenB);
//
//         Assert.IsTrue(bCompleted);
//         Object.Destroy(targetB);
//     }
//
//     // ===== Complex combinations =====
//
//     [UnityTest]
//     public IEnumerator KillWhenStopped_And_CompleteWhenKilled_FiresOnCompleteNotOnStop()
//     {
//         // Stop() routes through Kill() when KillWhenStopped is set, so OnStop should never fire —
//         // only OnComplete, and only because CompleteWhenKilled is also set.
//         var flags = new TweenSettings.TweenOptions(startAutomatically: true, killWhenStopped: true, completeWhenKilled: true);
//         var settings = new TweenSettings(0.5f, EasingType.Linear, flags: flags);
//
//         bool completed = false;
//         bool stopped = false;
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings, () => completed = true);
//         tween.OnStop += () => stopped = true;
//         yield return null;
//
//         tween.Stop();
//
//         Assert.IsFalse(tween.IsAlive);
//         Assert.IsTrue(completed);
//         Assert.IsFalse(stopped);
//     }
//
//     [UnityTest]
//     public IEnumerator KillWhenStopped_TakesPriorityOverResetWhenStopped()
//     {
//         // Both flags set: Stop() checks KillWhenStopped first and returns immediately,
//         // so ResetWhenStopped's revive-on-stop behavior should never run.
//         var flags = new TweenSettings.TweenOptions(startAutomatically: true, killWhenStopped: true, resetWhenStopped: true);
//         var settings = new TweenSettings(0.5f, EasingType.Linear, flags: flags);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         yield return null;
//
//         tween.Stop();
//
//         Assert.IsFalse(tween.IsAlive); // killed, not revived
//     }
//
//     [UnityTest]
//     public IEnumerator ResetWhenStopped_PingPong_InterruptedThenRestartsCleanly()
//     {
//         _target.transform.position = new Vector3(2f, 0f, 0f);
//
//         var flags = new TweenSettings.TweenOptions(startAutomatically: true, resetWhenStopped: true);
//         var settings = new TweenSettings(0.1f, EasingType.Linear, flags: flags,
//             loopingType: TweenSettings.LoopType.PingPong, loopCount: 2);
//
//         int loopCompleteCount = 0;
//         bool completed = false;
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings, () => completed = true);
//         tween.OnLoopComplete += () => loopCompleteCount++;
//
//         yield return null; // barely into the first leg, nowhere near a loop completion
//         tween.Stop();       // triggers Reset(), which restarts automatically since StartAutomatically is set
//
//         Assert.IsTrue(tween.IsAlive);
//         Assert.IsTrue(tween.IsRunning);
//
//         yield return WaitForCompletion(tween);
//
//         // A fresh, uninterrupted run should still produce a full LoopCount-worth of loop completions.
//         Assert.IsTrue(completed);
//         Assert.AreEqual(2, loopCompleteCount);
//     }
//
//     [UnityTest]
//     public IEnumerator InvertStartingDirection_With_PingPong_EndsAtTargetInsteadOfStart()
//     {
//         Vector3 startPos = _mover.transform.position;
//         _target.transform.position = new Vector3(3f, 0f, 0f);
//
//         var flags = new TweenSettings.TweenOptions(invertStartingDirection: true, startAutomatically: true);
//         var settings = new TweenSettings(0.1f, EasingType.Linear, flags: flags,
//             loopingType: TweenSettings.LoopType.PingPong, loopCount: 1);
//
//         int loopCompleteCount = 0;
//         bool completed = false;
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings, () => completed = true);
//         tween.OnLoopComplete += () => loopCompleteCount++;
//
//         yield return null;
//         // first leg runs backward (target -> start) because of the inverted starting direction
//         float distToTarget = Vector3.Distance(_mover.transform.position, _target.transform.position);
//         float distToStart = Vector3.Distance(_mover.transform.position, startPos);
//         Assert.Less(distToTarget, distToStart);
//
//         yield return WaitForCompletion(tween);
//
//         Assert.IsTrue(completed);
//         Assert.AreEqual(1, loopCompleteCount);
//         // normal PingPong ends back at the start; inverting the starting leg flips that, ending at the target instead
//         Assert.AreEqual(_target.transform.position, _mover.transform.position);
//     }
//
//     [UnityTest]
//     public IEnumerator IgnoreInvertEasing_With_InvertStartingDirection_ChangesFirstLegEasing()
//     {
//         // With InvertStartingDirection, the tween starts with isForwards == false, so
//         // (!isForwards && IgnoreInvertEasing) depends purely on the second flag from frame one.
//         var flagsIgnore = new TweenSettings.TweenOptions(invertStartingDirection: true, ignoreInvertEasing: true);
//         var settingsIgnore = new TweenSettings(0.1f, EasingType.InQuad, flags: flagsIgnore);
//
//         var flagsNormal = new TweenSettings.TweenOptions(invertStartingDirection: true, ignoreInvertEasing: false);
//         var settingsNormal = new TweenSettings(0.1f, EasingType.InQuad, flags: flagsNormal);
//
//         var valuesIgnore = new List<float>();
//         var valuesNormal = new List<float>();
//
//         Tween tweenIgnore = TweenManager.Value(_mover, 0f, 1f, settingsIgnore, v => valuesIgnore.Add(v));
//         tweenIgnore.Start();
//         yield return WaitForCompletion(tweenIgnore);
//
//         Tween tweenNormal = TweenManager.Value(_mover, 0f, 1f, settingsNormal, v => valuesNormal.Add(v));
//         tweenNormal.Start();
//         yield return WaitForCompletion(tweenNormal);
//
//         CollectionAssert.AreNotEqual(valuesIgnore, valuesNormal);
//     }
//
//     [UnityTest]
//     public IEnumerator Pause_DuringDelay_FreezesDelayCountdown()
//     {
//         _target.transform.position = new Vector3(2f, 0f, 0f);
//         Vector3 startPos = _mover.transform.position;
//
//         var settings = new TweenSettings(0.15f, EasingType.Linear, delayTime: 0.2f);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         tween.Start();
//         yield return null; // still inside the delay window, elapsed delay is ~0
//
//         bool unpaused = false;
//         tween.Pause(0.2f, () => unpaused = true);
//
//         // total real time here exceeds the original delay, but the delay clock never ran while paused
//         yield return new WaitForSeconds(0.25f);
//         Assert.AreEqual(startPos, _mover.transform.position);
//         Assert.IsTrue(unpaused);
//
//         yield return WaitForCompletion(tween);
//         Assert.AreEqual(_target.transform.position, _mover.transform.position);
//     }
//
//     [UnityTest]
//     public IEnumerator IgnoreTimeScale_With_TimedPause_ResumesOnUnscaledTime()
//     {
//         _target.transform.position = new Vector3(2f, 0f, 0f);
//
//         var flags = new TweenSettings.TweenOptions(ignoreTimeScale: true);
//         var settings = new TweenSettings(0.3f, EasingType.Linear, flags: flags);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//         tween.Start();
//         yield return null;
//
//         bool unpaused = false;
//         tween.Pause(0.1f, () => unpaused = true);
//
//         Time.timeScale = 0f; // pause countdown uses the same IgnoreTimeScale-aware deltaTime as the main update
//         yield return new WaitForSecondsRealtime(0.15f); // real time, unaffected by timeScale
//
//         Assert.IsTrue(unpaused);
//         Assert.IsFalse(tween.IsPaused);
//
//         Time.timeScale = 1f;
//         yield return WaitForCompletion(tween);
//         Assert.AreEqual(_target.transform.position, _mover.transform.position);
//     }
//
//     [UnityTest]
//     public IEnumerator Repeat_With_HangTime_And_KillWhenStopped_StopsDuringHangWindow()
//     {
//         _target.transform.position = new Vector3(1f, 0f, 0f);
//
//         var flags = new TweenSettings.TweenOptions(startAutomatically: true, killWhenStopped: true);
//         var settings = new TweenSettings(0.1f, EasingType.Linear, flags: flags,
//             loopingType: TweenSettings.LoopType.Repeat, loopCount: 3, hangTime: 0.3f);
//
//         Tween tween = _mover.transform.IT_Move(_target.transform, settings);
//
//         yield return new WaitForSeconds(0.12f); // past the first loop, now sitting in the hang window
//         Assert.IsTrue(tween.IsAlive);
//
//         tween.Stop(); // KillWhenStopped should short-circuit the hang wait entirely
//
//         Assert.IsFalse(tween.IsAlive);
//     }
//
//     [UnityTest]
//     public IEnumerator Overshoot_OutBack_With_InvertStartingDirection_StillOvershoots()
//     {
//         var flags = new TweenSettings.TweenOptions(invertStartingDirection: true);
//         var settings = new TweenSettings(0.1f, EasingType.OutBack, 1.70158f, flags: flags);
//
//         var values = new List<float>();
//         Tween tween = TweenManager.Value(_mover, 0f, 1f, settings, v => values.Add(v));
//         tween.Start();
//
//         yield return WaitForCompletion(tween);
//
//         float maxValue = float.MinValue;
//         foreach (float v in values) maxValue = Mathf.Max(maxValue, v);
//
//         Assert.Greater(maxValue, 1f); // overshoot still occurs, even traversing the curve in reverse
//         Assert.AreEqual(0f, values[values.Count - 1], 0.05f); // reversed direction ends near 0, not near 1
//     }
//
//     [UnityTest]
//     public IEnumerator TwoConcurrentTweens_DifferentSettings_DoNotInterfere()
//     {
//         GameObject moverB = new GameObject("MoverB");
//         GameObject targetB = new GameObject("TargetB");
//         targetB.transform.position = new Vector3(5f, 0f, 0f);
//         _target.transform.position = new Vector3(2f, 0f, 0f);
//
//         var flagsA = new TweenSettings.TweenOptions(startAutomatically: true);
//         var settingsA = new TweenSettings(0.1f, EasingType.Linear, flags: flagsA,
//             loopingType: TweenSettings.LoopType.Repeat, loopCount: 2);
//
//         var flagsB = new TweenSettings.TweenOptions(startAutomatically: true, invertStartingDirection: true);
//         var settingsB = new TweenSettings(0.1f, EasingType.Linear, flags: flagsB,
//             loopingType: TweenSettings.LoopType.PingPong, loopCount: 1);
//
//         bool completedA = false;
//         bool completedB = false;
//         Tween tweenA = _mover.transform.IT_Move(_target.transform, settingsA, () => completedA = true);
//         Tween tweenB = moverB.transform.IT_Move(targetB.transform, settingsB, () => completedB = true);
//
//         yield return WaitForCompletion(tweenA);
//         yield return WaitForCompletion(tweenB);
//
//         Assert.IsTrue(completedA);
//         Assert.IsTrue(completedB);
//         Assert.AreEqual(_target.transform.position, _mover.transform.position); // Repeat ends at target
//         Assert.AreEqual(targetB.transform.position, moverB.transform.position); // PingPong+invert ends at target too
//
//         Object.Destroy(moverB);
//         Object.Destroy(targetB);
//     }
// }