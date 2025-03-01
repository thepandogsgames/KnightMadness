namespace Code.Board
{
    public interface IBoardPiece
    {
        public IBoardCell CurrentCell { get; set; }

        public void Reset();
        public void Eaten();
    }
}
