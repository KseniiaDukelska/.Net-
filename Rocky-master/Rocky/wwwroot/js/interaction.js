function logInteraction(productId, interactionType, interactionValue = null) {
    $.ajax({
        url: '/api/interactions/log',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            productId: productId,
            interactionType: interactionType,
            interactionValue: interactionValue
        }),
        success: function (response) {
            console.log('Interaction logged successfully');
        },
        error: function (error) {
            console.error('Error logging interaction:', error);
        }
    });
}
