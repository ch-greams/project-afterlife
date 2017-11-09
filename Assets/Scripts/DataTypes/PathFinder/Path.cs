using System.Collections;
using System.Collections.Generic;


public class Path<T>: IEnumerable<T>
{
	public T lastStep            { get; private set; }
	public Path<T> previousSteps { get; private set; }
    public double totalCost      { get; private set; }
    public int totalSteps        { get; private set; }

	private Path(T lastStep, Path<T> previousSteps, double totalCost, int totalSteps)
    {
        this.lastStep = lastStep;
        this.previousSteps = previousSteps;
        this.totalCost = totalCost;
        this.totalSteps = totalSteps;
    }

	public Path(T start) : this(start, null, 0, 0) { }

	public Path<T> AddStep(T step, double stepCost)
    {
        return new Path<T>(step, this, totalCost + stepCost, totalSteps + 1);
    }

	public IEnumerator<T> GetEnumerator()
    {
        for (Path<T> p = this; p != null; p = p.previousSteps)
        {
            yield return p.lastStep;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}