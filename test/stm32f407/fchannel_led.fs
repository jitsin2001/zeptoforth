\ Copyright (c) 2020 Travis Bemann
\
\ Permission is hereby granted, free of charge, to any person obtaining a copy
\ of this software and associated documentation files (the "Software"), to deal
\ in the Software without restriction, including without limitation the rights
\ to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
\ copies of the Software, and to permit persons to whom the Software is
\ furnished to do so, subject to the following conditions:
\ 
\ The above copyright notice and this permission notice shall be included in
\ all copies or substantial portions of the Software.
\ 
\ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
\ IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
\ FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
\ AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
\ LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
\ OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
\ SOFTWARE.

\ Set up the wordlist order
forth-wordlist internal-wordlist task-wordlist fchan-wordlist led-wordlist
5 set-order
forth-wordlist set-current

\ Allot the channels
fchan-size buffer: orange-fchan
fchan-size buffer: green-fchan
fchan-size buffer: blue-fchan
fchan-size buffer: red-fchan

\ Initialize the channels
orange-fchan init-fchan
green-fchan init-fchan
blue-fchan init-fchan
red-fchan init-fchan

\ The led structure
begin-structure led-header-size
  \ The before fast channel
  field: before-fchan

  \ The after fast channel
  field: after-fchan

  \ The on xt
  field: on-xt

  \ The off xt
  field: off-xt

  \ The delay
  field: delay
end-structure

\ The inner loop of an led handler
: do-led ( -- )
  does> begin
    dup before-fchan @ recv-fchan-cell drop
    dup on-xt @ execute
    dup delay @ ms
    dup off-xt @ execute
    dup after-fchan @ 0 swap send-fchan-cell
    pause
  again
;

\ Create an led task
: make-led ( before-fchan after-fchan on-xt off-xt delay "name" -- )
  s" " <builds-with-name 4 roll , 3 roll , rot , swap , , do-led
  latest >body 256 256 256 spawn constant
;

\ Create the led tasks
orange-fchan green-fchan ' led-orange-on ' led-orange-off 100
make-led orange-task
green-fchan blue-fchan ' led-green-on ' led-green-off 100 make-led green-task
blue-fchan red-fchan ' led-blue-on ' led-blue-off 100 make-led blue-task
red-fchan orange-fchan ' led-red-on ' led-red-off 100 make-led red-task

\ Initialize the blinky
: init-blinky ( -- )
  orange-task enable-task
  green-task enable-task
  blue-task enable-task
  red-task enable-task
  pause
  0 orange-fchan send-fchan-cell
  pause
;
