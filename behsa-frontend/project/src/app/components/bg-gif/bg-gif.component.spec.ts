import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BgGifComponent } from './bg-gif.component';

describe('BgGifComponent', () => {
  let component: BgGifComponent;
  let fixture: ComponentFixture<BgGifComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BgGifComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BgGifComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
