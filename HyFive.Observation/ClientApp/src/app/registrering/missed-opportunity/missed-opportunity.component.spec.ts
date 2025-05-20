import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MissedOpportunityComponent } from './missed-opportunity.component';

describe('MissedOpportunityComponent', () => {
  let component: MissedOpportunityComponent;
  let fixture: ComponentFixture<MissedOpportunityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MissedOpportunityComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MissedOpportunityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
