namespace GameDemo;
public partial class AppLogic {
  public partial record State {
    public record RestartingGame : State, IGet<Input.FadeOutFinished> {
      public RestartingGame() {
        OnEnter<RestartingGame>(
          (previous) => Context.Output(new Output.FadeOut())
        );
        OnExit<RestartingGame>(
          (next) => {
            Context.Output(new Output.RemoveExistingGame());
            Context.Output(new Output.LoadGame());
          }
        );
      }

      public IState On(Input.FadeOutFinished input) =>
        new PlayingGame();
    }
  }
}
