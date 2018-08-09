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
    route: Route;
    trainArrivals: TrainArrival[];
    stations: StationObject[];
    accessibleStations: StationObject[];
    loading: boolean;
    currentDateTime: string;
    fromStationSelected: string;
    toStationSelected: string;
    isOutbound: boolean;
}

interface StationObject
{
    value: string;
    label: string;
}
 
interface Route
{
    from_station_alert_text: string;
    from_station_alert_memo: string;
    to_station_alert_text: string;
    to_station_alert_memo: string;
}

interface TrainArrival
{
    origin_departure_time: string; 
    origin_name: string; 
    dest_arrival_time: string; 
    dest_name: string; 
    arrives_in_min: string; 
    time_on_train: string; 
    late_by_min: string; 
    alert: string;
    route_id: string;
    trip_id: string;
    tripURL: string;
    description: string;
};


var divStyle = { 
    margin: "20px" 
};

export class MetraSchedule extends React.Component<RouteComponentProps<{}>, FetchDataScheduleState> {

    constructor() {
        super();
         
        this.state = {
            route: null, trainArrivals: [], stations: [], accessibleStations: [],
            loading: false, currentDateTime: '',
            fromStationSelected: null, toStationSelected: null, isOutbound: true
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

        this.loadCollections();
        this.loadStations();

        //Repeat loading every 30 seconds
        //setInterval(() => {
        //    this.loadData();
        //}, 30000);

        
        //this.loadData();
    }

    loadCollections()
    {
        try {
            fetch('api/Metra/LoadInitialData'); 
            console.log('Collections loaded!')
        } catch (e) {
            //console.log(e);
        }
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
        try {
            if (this.state.fromStationSelected == null) return;
            if (this.state.toStationSelected == null) return;

            console.log(`Station selected:`, this.state.fromStationSelected['value']);

            this.setState({ loading: true });

            fetch('api/Metra/GetScheduleData?FromStationAbbrev=' + this.state.fromStationSelected['value']
                + '&ToStationAbbrev=' + this.state.toStationSelected['value']
                + '&Direction=' + this.state.isOutbound, {
            })
                .then(response => response.json() as Promise<TrainArrival[]>)
                .then(data => {
                    this.setState({ trainArrivals: data, loading: false, currentDateTime: 'Updated: ' + new Date().toLocaleString() });
                });
             
        } catch (e) {
            //console.log(e);
        }
    }

    loadAccessibleStations()
    {
        if (this.state.fromStationSelected == null) return;

        this.setState({ accessibleStations: []});

        fetch('api/Metra/GetAccessibleStations?StationAbbrev=' + this.state.fromStationSelected['value'] + '&IsOutbound=' + Number(this.state.isOutbound), {
        })
            .then(response => response.json() as Promise<StationObject[]>)
            .then(data => {
                this.setState({ accessibleStations: data });
            })
            .then(data => { 
                if (this.state.accessibleStations.length == 1)
                {
                    console.log('Selecting an option');
                    this.setState({ toStationSelected: this.state.accessibleStations[0]['value']});
                }
            }); 
    }

    direction_OnChanged = (isOutbound) => { 
         
        this.setState(
            { isOutbound: isOutbound, loading: true}, 
            () => this.loadAccessibleStations()
        );
         
        console.log('Direction is outbound? ' + isOutbound);

        //this.loadAccessibleStations();
    }

    fromStation_OnSelected = (fromStationSelected) => {
         
        this.setState(
            { fromStationSelected: fromStationSelected, loading: true },
            () => this.loadAccessibleStations()
        );
         
        console.log(`Option selected:`, fromStationSelected);
    }

    toStation_OnSelected = (toStationSelected) => {
         
        this.setState(
            { toStationSelected: toStationSelected},
            () => this.loadData()
        );

        console.log(`Option selected:`, toStationSelected); 
    }
     
    arrowRenderer()
    {
        return (
            <span>+</span>
        );
    }

    public render() {

        const { fromStationSelected } = this.state;
        const { toStationSelected } = this.state;
        const { isOutbound } = this.state;

        //console.debug(this.state.routes);
        let contents = this.state.loading
            ? <div><em>Loading...</em></div>
            : MetraSchedule.renderForecastsTable(this.state.trainArrivals);

        return <div >
            <h1>Metra Scheduled Train Routes</h1>
            <div>  
                <label htmlFor="normal-switch">
                    <span>Inbound       </span>
                    <Switch 
                        onChange={this.direction_OnChanged}
                        checked={this.state.isOutbound}
                        uncheckedIcon={false}
                        offColor="#2633e6"
                        onColor="#e62633"
                        checkedIcon={false}
                        id="normal-switch"
                    />
                    <span>       Outbound</span>
                </label>

            </div>
            <div>
                <Label>From: </Label>
                <Select  
                    value={fromStationSelected}
                    options={this.state.stations}
                    onChange={this.fromStation_OnSelected}
                    loading={this.state.loading}
                    arrowRenderer={this.arrowRenderer}
                />
            </div>
            <div>
                <Label>To: </Label>
                <Select
                    value={toStationSelected}
                    options={this.state.accessibleStations}
                    onChange={this.toStation_OnSelected}
                    loading={this.state.loading}
                    arrowRenderer={this.arrowRenderer}
                    
                />
            </div>
            <div>{this.state.currentDateTime}</div>
            {contents}
        </div>;
    }
     
    private static renderForecastsTable(trainArrivals: TrainArrival[]) {
        return  <table className='table'>
                    <thead>
                        <tr>
                            <th>Departing Station</th>
                            <th>Arriving Station</th>
                            <th>Departing At</th>                           
                            <th>Arriving At</th>
                            <th>Status</th>
                            <th>TripID</th>
                        </tr>
                    </thead>
                    <tbody>
                            {trainArrivals.map((tr, index) => 
                            <tr key={index}>
                                <td>{tr.origin_name}</td>
                                <td>{tr.dest_name}</td>
                                <td><b>{tr.origin_departure_time}</b></td> 
                                <td><b>{tr.dest_arrival_time}</b></td>
                                <td>{tr.alert}</td>
                                <td><a href={tr.tripURL}>{tr.trip_id}</a></td>
                            </tr>
                        )}
                    </tbody>
                    </table>;
    }

}
