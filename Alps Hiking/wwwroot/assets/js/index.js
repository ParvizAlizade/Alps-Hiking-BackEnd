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




let value = 0;
let value1 = 0;
let value2 = 0;
let value3 = 0;
let isScrolledToSection = false;
const section = document.getElementById('statistic');

window.addEventListener('scroll', function () {
  if (!isScrolledToSection && window.innerHeight + window.pageYOffset >= section.offsetTop + section.offsetHeight - 260) {
    isScrolledToSection = true;
    setInterval(() => {
      if (value3 < 3500) {
        value3 += 4;
      }
      document.getElementById("days").innerHTML = value3 + '+';
    }, 1);
    setInterval(() => {
      if (value2 < 85) {
        value2 += 1;
      }
      document.getElementById("places").innerHTML = value2 + '+';
    }, 37);

    setInterval(() => {
      if (value1 < 3000) {
        value1 += 8;
      }
      document.getElementById("hikers").innerHTML = value1 + '+';
    }, .5);

    setInterval(() => {
      if (value < 10000) {
        value += 14;
      }
      document.getElementById("miles").innerHTML = value + '+';
    }, 1);

  }
});