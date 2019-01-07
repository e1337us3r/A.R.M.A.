
public class ToggleElectricRail : ControlButton
{
    public int mode = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    //different modes correspond to buttons for nonelectric (0), high speed electric (2) and normal electric (1)
    //the 'active' version of the button do not have this script, since this is threeway toggle, not a on-off toggle
    override public void DoStuff()
    {
        switch (mode)
        {
            case 0:
                AccessMain().ToggleElectricRail(false);
                AccessMain().elecspeedhigh = 0;
                break;
            case 1:
                AccessMain().ToggleElectricRail(true);
                AccessMain().ToggleElectricRailSpeed(false);
                AccessMain().elecspeedhigh = 1;
                break;
            case 2:
                AccessMain().ToggleElectricRail(true);
                AccessMain().ToggleElectricRailSpeed(true);
                AccessMain().elecspeedhigh = 2;
                break;
        }
    }
}
