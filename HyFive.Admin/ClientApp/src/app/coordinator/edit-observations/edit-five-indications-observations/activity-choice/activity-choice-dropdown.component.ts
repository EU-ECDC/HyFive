import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ActivityType} from "../../../../models/api/ActivityType";
import {ActivityTypeService} from "../../../../services/data/activity-type.service";

@Component({
  selector: 'app-activity-choice-dropdown',
  templateUrl: './activity-choice-dropdown.component.html'
})
export class ActivityChoiceDropdownComponent implements OnInit{

  @Input() activityTypeId: number;
  @Output() activityTypeSelected: EventEmitter<ActivityType> = new EventEmitter<ActivityType>();
  selectedActivityType: ActivityType;
  activityTypes: ActivityType[] = [];

  selectedActivityTypeId: string;

  constructor(private readonly activityTypeService: ActivityTypeService){}

  ngOnInit() {
    this.activityTypeService.getActivityTypes().subscribe(
      (activityTypes) => {
      this.activityTypes = activityTypes;
      this.selectedActivityTypeId = this.activityTypeId+'';
      this.selectActivityType();
      }
    );
  }

  selectedActivityTypeChanged() {
    this.selectActivityType();
    this.activityTypeSelected.emit(this.selectedActivityType);
  }

  selectActivityType() {
    if (this.activityTypes){
      this.selectedActivityType = this.activityTypes[this.activityTypes.map(r => r.id).indexOf(Number.parseInt(this.selectedActivityTypeId))];
    }
  }
}
