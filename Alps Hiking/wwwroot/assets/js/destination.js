$('#search').on('keyup', function() {
    var value = $(this).val();
    var patt = new RegExp(value, "i");
  
    $('.tab_content').find('.col-lg-4').each(function() {
      var $table = $(this);
      
      if (!($table.find('.featured-item').text().search(patt) >= 0)) {
        $table.not('.featured-item').hide();
      }
      if (($table.find('.col-lg-4').text().search(patt) >= 0)) {
        $(this).show();
        document.getElementById('not_found').style.opacity = '0';
      } else {
        document.getElementById("not_found").innerHTML = " Product not found..";
        document.getElementById('not_found').style.display = 'block';
      }
    });
});

