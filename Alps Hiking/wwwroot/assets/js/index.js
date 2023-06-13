$('.announce').slick({
  slidesToShow: 5,
  autoplay: true,
  autoplaySpeed: 0,
  speed: 6000,
  center: false,
  dots: false,
  cssEase: 'linear',
  easing: 'easeOutElastic',
  responsive: [
    {
      breakpoint: 1024,
      settings: {
        slidesToShow: 2,
        slidesToScroll: 1,
        infinite: true,
        dots: true
      }
    },
    {
      breakpoint: 400,
      settings: {
        slidesToShow: 2,
        slidesToScroll: 1
      }
    }
  ]
});

$('.allreviews').slick({
  infinite: true,
  autoplay: true,
  autoplaySpeed: 3000,
  speed: 2000,
  slidesToShow: 4,
  slidesToScroll: 2,
  responsive: [
    {
      breakpoint: 1024,
      settings: {
        slidesToShow: 2,
        slidesToScroll: 1,
        infinite: true,
        dots: true
      }
    },
    {
      breakpoint: 730,
      settings: {
        slidesToShow: 1,
        slidesToScroll: 1
      }
    }
  ]
});



window.addEventListener("scroll", function () {
  var video = document.getElementById("myVideo");
  var videoPosition = video.getBoundingClientRect().top;
  var windowHeight = window.innerHeight;
  if (videoPosition < windowHeight) {
    video.play();
  }
});



