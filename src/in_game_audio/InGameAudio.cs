namespace GameDemo;

using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.LogicBlocks;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

[SuperNode(typeof(AutoNode), typeof(Dependent))]
public partial class InGameAudio : Node {
  public override partial void _Notification(int what);

  #region Nodes

  [Node] public IAudioStreamPlayer CoinCollected { get; set; } = default!;
  [Node] public IAudioStreamPlayer Bounce { get; set; } = default!;
  [Node] public IAudioStreamPlayer PlayerDied { get; set; } = default!;
  [Node] public IAudioStreamPlayer PlayerJumped { get; set; } = default!;
  [Node] public IDimmableAudioStreamPlayer MainMenuMusic { get; set; } = default!;
  [Node] public IDimmableAudioStreamPlayer GameMusic { get; set; } = default!;

  #endregion Nodes

  #region Dependencies

  [Dependency] public IAppRepo AppRepo => DependOn<IAppRepo>();

  [Dependency] public IGameRepo GameRepo => DependOn<IGameRepo>();

  #endregion Dependencies

  #region State

  public IInGameAudioLogic InGameAudioLogic { get; set; } = default!;

  public Logic<InGameAudioLogic.IState, Func<object, InGameAudioLogic.IState>, InGameAudioLogic.IState,
    Action<InGameAudioLogic.IState?>>.IBinding InGameAudioBinding { get; set; } = default!;

  #endregion State

  public void Setup() => InGameAudioLogic = new InGameAudioLogic(AppRepo, GameRepo);

  public void OnResolved() {
    InGameAudioBinding = InGameAudioLogic.Bind();

    InGameAudioBinding
      .Handle<InGameAudioLogic.Output.PlayCoinCollected>(
        _ => CoinCollected.Play()
      )
      .Handle<InGameAudioLogic.Output.PlayBounce>(
        _ => Bounce.Play()
      )
      .Handle<InGameAudioLogic.Output.PlayPlayerDied>(
        _ => PlayerDied.Play()
      )
      .Handle<InGameAudioLogic.Output.PlayJump>(
        _ => PlayerJumped.Play()
      )
      .Handle<InGameAudioLogic.Output.PlayMainMenuMusic>(
        _ => StartMainMenuMusic()
      )
      .Handle<InGameAudioLogic.Output.PlayGameMusic>(
        _ => StartGameMusic()
      );

    InGameAudioLogic.Start();
  }

  public void OnExitTree() {
    InGameAudioLogic.Stop();
    InGameAudioBinding.Dispose();
  }

  public void StartMainMenuMusic() {
    GameMusic.FadeOut();
    MainMenuMusic.Stop();
    MainMenuMusic.FadeIn();
  }

  public void StartGameMusic() {
    MainMenuMusic.FadeOut();
    GameMusic.Stop();
    GameMusic.FadeIn();
  }
}
