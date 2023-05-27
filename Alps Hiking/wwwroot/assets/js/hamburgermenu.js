$(document).ready(function(){
  $('.container ul.toggle').click(function(){
      $(this).toggleClass('active');
      $('.container .sidebar').toggleClass('active');
  })
})