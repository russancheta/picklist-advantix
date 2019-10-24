import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenSoComponent } from './openso.component';

describe('OpensoComponent', () => {
  let component: OpenSoComponent;
  let fixture: ComponentFixture<OpenSoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OpenSoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OpenSoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
