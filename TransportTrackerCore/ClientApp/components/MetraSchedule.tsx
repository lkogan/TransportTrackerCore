import * as React from 'react'; 
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { Button} from 'react-bootstrap';
import { ButtonToolbar } from 'react-bootstrap';
import { DropdownButton } from 'react-bootstrap';
import { MenuItem } from 'react-bootstrap';
import { ToggleButtonGroup } from 'react-bootstrap';
import { Label } from 'react-bootstrap';
import { ToggleButton } from 'react-bootstrap';
import Select from 'react-select';
//import { ReactSwitchProps } from "react-switch";
import { Component } from "react";
import Switch from "react-switch";

interface FetchDataScheduleState {
    routes: StopOnTrip[];
    stations: StationObject[];
    accessibleStations: StationObject[];
    loading: boolean;
    currentDateTime: string;
    fromStationSelected: string;
    toStationSelected: string;
    directionSelected: boolean;
    checked: boolean;
}

interface StationObject
{
    value: string;
    label: string;
}

interface StopOnTrip {
    trip_id: string;
    arrival_time: string;
    departure_time: string;
    stop_id: string;
    stop_sequence: number;
    pickup_type: number;
    drop_off_type: number;
    center_boarding: number;
    south_boarding: number;
    bikes_allowed: number;
    notice: number;
} 
  
 
var divStyle = { 
    margin: "20px" 
};

export class MetraSchedule extends React.Component<RouteComponentProps<{}>, FetchDataScheduleState> {

    constructor() {
        super();
         
        this.state = {
            routes: [], stations: [], accessibleStations: [],
            loading: true, currentDateTime: '',
            fromStationSelected: null, toStationSelected: null, directionSelected: false, checked: true
        }; 
          
        //this.loadData();

        

        //this.loadStations();
    }

    componentWillMount()
    { 
        
    }

    componentDidMount()
    {
        //fetch('api/Metra/LoadInitialData');

        this.loadStations();

        //Repeat loading every 30 seconds
        //setInterval(() => {
        //    this.loadData();
        //}, 30000);

        
        //this.loadData();
    }

    loadStations()
    {
        try {
            fetch('api/Metra/GetStations')
                .then(response => response.json() as Promise<StationObject[]>)
                .then(data => {
                    this.setState({ stations: data });
                });

        } catch (e) {
            //console.log(e);
        }
    }
     
    loadData()
    {
        try
        {  
            if (this.state.fromStationSelected == null) return;
            if (this.state.toStationSelected == null) return;
             
            console.log(`Station selected:`, this.state.fromStationSelected['value']);

            fetch('api/Metra/GetScheduleData?FromStationAbbrev=' + this.state.fromStationSelected['value']
                + '&ToStationAbbrev=' + this.state.toStationSelected['value']
                + '&Direction=' + this.state.directionSelected, {
            })
                .then(response => response.json() as Promise<StopOnTrip[]>)
                .then(data => {
                    this.setState({ routes: data, loading: false, currentDateTime: 'Updated: ' + new Date().toLocaleString() });
                });
             
        } catch (e) {
            //console.log(e);
        }
    }
     
    fromStation_OnSelected = (fromStationSelected) => {
        this.setState({ fromStationSelected: fromStationSelected});
         
        fetch('api/Metra/GetAccessibleStations?StationAbbrev=' + fromStationSelected.value + '&direction=' + this.state.directionSelected, {
        })
        .then(response => response.json() as Promise<StationObject[]>)
        .then(data => {
            this.setState({ accessibleStations: data });
        });


        console.log(`Option selected:`, fromStationSelected);
    }

    toStation_OnSelected = (toStationSelected) => {
        this.setState({ toStationSelected: toStationSelected });
        console.log(`Option selected:`, toStationSelected);

        this.loadData();
    }

    direction_OnChanged = (directionSelected) => {
        this.setState({ directionSelected: directionSelected}); 
        console.log('Changed direction:' + directionSelected);
    }

    handleChange(checked) {
        this.setState({ checked: checked });
    }

    public render() {

        const { fromStationSelected } = this.state;
        const { toStationSelected } = this.state;
        const { directionSelected } = this.state;

        //console.debug(this.state.routes);
        let contents = this.state.loading
            ? <div><em>Loading...</em></div>
            : MetraSchedule.renderForecastsTable(this.state.routes);

        return <div >
            <h1>Metra Scheduled Train Routes</h1>
            <div>
                <Label>Direction: </Label>
                <ToggleButtonGroup style={divStyle} type="radio" name="direction"
                    value={this.state.directionSelected} onChange={this.direction_OnChanged}>
                    <ToggleButton checked value={0}>Outbound</ToggleButton>
                    <ToggleButton value={1}>Inbound</ToggleButton>
                </ToggleButtonGroup>  

                <label htmlFor="normal-switch">
                    <span>Inbound</span>
                    <Switch 
                        onChange={this.handleChange}
                        checked={this.state.checked}
                        id="normal-switch"
                    />
                    <span>Outbound</span>
                </label>

            </div>
            <div>
                <Label>From: </Label>
                <Select  
                    defaultvalue={this.state.stations[0]}
                    value={fromStationSelected}
                    options={this.state.stations}
                    onChange={this.fromStation_OnSelected}
                />
            </div>
            <div>
                <Label>To: </Label>
                <Select
                    defaultvalue={this.state.accessibleStations[0]}
                    value={toStationSelected}
                    options={this.state.accessibleStations}
                    onChange={this.toStation_OnSelected}
                    
                />
            </div>
            <div>{this.state.currentDateTime}</div>
            {contents}
        </div>;
    }
     
    private static renderForecastsTable(routes: StopOnTrip[]) {
        return  <table className='table'>
                    <thead>
                        <tr>
                            <th>DepartingFrom</th>
                            <th>Departs At</th>
                            <th>TripID</th>
                        </tr>
                    </thead>
                    <tbody>
                        {routes.map((route, index) => 
                            <tr key={index}>
                                <td>{route.stop_id}</td>
                                <td>{route.arrival_time}</td>
                                <td>{route.trip_id}</td>
                            </tr>
                        )}
                    </tbody>
                    </table>;
    }

}
