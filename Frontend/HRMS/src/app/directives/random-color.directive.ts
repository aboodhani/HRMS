import { Directive, ElementRef } from '@angular/core';

@Directive({
  selector: '[appRandomColor]'
})
export class RandomColorDirective {

  constructor(private element: ElementRef) { 
    element.nativeElement.style.backgroundColor = 'red';
  }

}
