namespace GameDemo;
public partial class GameLogic {
  public interface IState : IStateLogic { }

  public record State : StateLogic, IState, IGet<Input.Initialize> {
    public State() {
      OnAttach(
        () => {
          var appRepo = Context.Get<IAppRepo>();
          appRepo.GameStarting += GameAboutToStart;
          appRepo.GamePaused += GamePaused;
          appRepo.GameResumed += GameResumed;
        }
      );

      OnDetach(() => {
        var appRepo = Context.Get<IAppRepo>();
        appRepo.GameStarting -= GameAboutToStart;
        appRepo.GamePaused -= GamePaused;
        appRepo.GameResumed -= GameResumed;
      });
    }

    public void GameAboutToStart() {
      Context.Output(new Output.ChangeToThirdPersonCamera());
    }

    public void GamePaused() => Context.Output(new Output.SetPauseMode(true));
    public void GameResumed() => Context.Output(new Output.SetPauseMode(false));

    public IState On(Input.Initialize input) {
      var appRepo = Context.Get<IAppRepo>();
      appRepo.OnNumCoinsAtStart(input.NumCoinsInWorld);
      return this;
    }
  }
}
